# ECommerce API Specification

## Authentication

### Overview
The API uses **JWT Bearer token authentication**. All requests (except auth endpoints) require an `Authorization: Bearer {token}` header.

### Endpoints

#### 1. Register
```
POST /api/auth/register
{
  "userName": "testuser",
  "email": "test@example.com",
  "password": "Test@123"
}

Response (200):
{
  "status": true,
  "data": {
    "id": "user-id",
    "email": "test@example.com",
    "token": "eyJhbGc...",
    "refreshToken": "abc123..."
  }
}
```

#### 2. Login
```
POST /api/auth/login
{
  "email": "test@example.com",
  "password": "Test@123"
}

Response (200):
{
  "status": true,
  "data": {
    "id": "user-id",
    "token": "eyJhbGc...",
    "refreshToken": "abc123..."
  }
}
```

#### 3. Refresh Token
```
POST /api/auth/refresh-token
{
  "refreshToken": "{{refreshToken}}"
}

Response (200):
{
  "status": true,
  "data": {
    "token": "eyJhbGc...",
    "refreshToken": "xyz789..."
  }
}

Use when: Access token expires
Returns: New access token
```

#### 4. Forgot Password
```
POST /api/auth/forgot-password
{
  "email": "test@example.com"
}

Response (200):
{
  "status": true,
  "message": "OTP sent to email"
}

Effect: Sends 6-digit OTP code to user's registered email
```

#### 5. Verify OTP
```
POST /api/auth/verify-otp
{
  "email": "test@example.com",
  "otp": "123456"
}

Response (200):
{
  "status": true,
  "message": "OTP verified"
}

Effect: Validates the OTP code sent to email
Must be verified before password reset
```

#### 6. Reset Password
```
POST /api/auth/reset-password
{
  "email": "test@example.com",
  "otp": "123456",
  "newPassword": "NewPassword@123",
  "confirmPassword": "NewPassword@123"
}

Response (200):
{
  "status": true,
  "message": "Password reset successful"
}

Prerequisites: 
- OTP must be verified first (Verify OTP endpoint)
- Passwords must match
- Passwords must meet security requirements
```

### Security Requirements
- Passwords: Minimum 8 characters (numbers, uppercase, lowercase, special chars recommended)
- OTP: Valid for 5 minutes from issue time
- Tokens: JWT tokens valid for 15-30 minutes
- Refresh Tokens: Valid for 7 days

### Auth Flow Sequence

**Normal Login:**
```
1. POST /auth/register OR /auth/login
2. Receive: token + refreshToken
3. Use token in Authorization header for all requests
4. When token expires → POST /auth/refresh-token
5. Get new token, continue
```

**Password Recovery:**
```
1. POST /auth/forgot-password (send OTP)
2. Check email for 6-digit code
3. POST /auth/verify-otp (verify OTP)
4. POST /auth/reset-password (set new password)
5. Login with new password
```

---

## System Architecture

### Stock Management System

The system supports **two types of products**:

#### 1. Products WITH Variants (`HasVariants = true`)
Examples: T-Shirts, Sneakers, Clothing (with size/color options)

**Stock Calculation:**
```
Product.StockQuantity = SUM(Variant.StockQuantity) for all variants
```

**Auto-Calculated:** 
- Set during variant creation/update
- Recalculated when variant stock changes
- Reset to 0 when last variant deleted

**Cart Behavior:**
- User MUST select a variant (size, color, etc.)
- Stock validated at **variant level**
- `productVariantId` is **REQUIRED**

#### 2. Products WITHOUT Variants (`HasVariants = false`)
Examples: Posters, Digital products, Unique items

**Stock Management:**
- Set manually at product level
- No variants needed

**Cart Behavior:**
- Add directly without variant selection
- Stock validated at **product level**
- `productVariantId` must be **NULL/omitted**

---

## Data Model

### Product
```
{
  id: int (PK),
  name: string,
  description: string,
  price: decimal(18,2),
  hasVariants: bool,              ← New: variant flag
  stockQuantity: int,             ← New: product-level stock
  isActive: bool,
  categoryId: int (FK),
  brandId: int (FK),
  createdAt: datetime,
  updatedAt: datetime
}
```

### ProductVariant
```
{
  id: int (PK),
  productId: int (FK),
  imageUrl: string,
  size: string,
  color: string,
  additionalPrice: decimal(18,2)?,
  stockQuantity: int,             ← Variant-level stock
  createdAt: datetime,
  updatedAt: datetime
}
```

### CartItem
```
{
  id: int (PK),
  quantity: int,
  productId: int (FK),            ← New: direct product ref
  productVariantId: int? (FK),    ← New: nullable
  unitPrice: decimal(18,2),       ← Snapshot price
  cartId: int (FK),
  rowVersion: byte[],             ← Concurrency control
  createdAt: datetime,
  updatedAt: datetime
}
```

---

## Business Rules

### 🛑 Rule 1: Variant Selection
```
IF Product.HasVariants = true
  THEN ProductVariantId is REQUIRED
  ELSE ProductVariantId must be NULL
```

**Validation Location:** CartItemsService.AddToCartAsync()
**Error:** "This product has/doesn't have variants..."

### 🛑 Rule 2: Stock Validation
```
IF Product.HasVariants = true
  THEN CheckStock(ProductVariant.StockQuantity)
  ELSE CheckStock(Product.StockQuantity)
```

**Validation Location:** CartItemsService methods
**Prevents:** Over-selling

### 🛑 Rule 3: Stock Recalculation
```
WHEN Variant.Create() OR Variant.Update(stock) OR Variant.Delete()
  THEN RecalculateProductStock(ProductId)
```

**Automatic Triggers:**
- `ProductVariantsService.CreateAsync()` - Mark as HasVariants=true
- `ProductVariantsService.UpdateAsync()` - If stock changed
- `ProductVariantsService.DeleteAsync()` - If last variant, set HasVariants=false

### 🛑 Rule 4: Price Snapshot
```
WHEN CartItem.Create()
  THEN UnitPrice = Product.Price + (ProductVariant.AdditionalPrice ?? 0)
```

**Purpose:** Cart totals remain consistent even if product prices change
**Location:** CartItemsService.AddToCartAsync()

### 🛑 Rule 5: Variant Deletion Protection
```
IF CartItems.Where(c => c.ProductVariantId = variantId).Count > 0
  THEN CANNOT Delete Variant
  ELSE Delete Allowed
```

**Prevents:** Orphaned cart items
**Error:** "Cannot delete variant with active cart items"

---

## API Workflows

### Workflow 1: Create Product with Variants (Admin)

```
Step 1: Create product
POST /api/products/create
{
  "name": "T-Shirt",
  "hasVariants": true,
  "stockQuantity": 0,           ← Leave as 0, will be calculated
  ...
}
→ productId = 1

Step 2: Add variants (multiple calls)
POST /api/products/1/variants
{ "size": "S", "color": "Blue", "stockQuantity": 50 }
→ Product.StockQuantity = 50, HasVariants = true

POST /api/products/1/variants
{ "size": "M", "color": "Blue", "stockQuantity": 60 }
→ Product.StockQuantity = 110, HasVariants = true

Step 3: Product ready for shopping
GET /api/products/1
→ StockQuantity = 110 (automatically calculated)
```

### Workflow 2: Customer Add to Cart (Variant Product)

```
Step 1: Browse product details
GET /api/products/1
→ HasVariants = true, StockQuantity = 110

Step 2: Get available variants
GET /api/products/1/variants
→ [
    { id: 1, size: "S", stockQuantity: 50 },
    { id: 2, size: "M", stockQuantity: 60 }
  ]

Step 3: Customer selects Medium Blue
POST /api/cart/items
{
  "productId": 1,
  "productVariantId": 2,        ← REQUIRED
  "quantity": 2
}
→ Checks: variant (id=2) has stock >= 2
→ UnitPrice = 29.99 + 0 = 29.99
→ CartItem created with snapshot price
```

### Workflow 3: Customer Add to Cart (Non-Variant Product)

```
Step 1: Browse product
GET /api/products/2
→ HasVariants = false, StockQuantity = 25

Step 2: Add to cart (no variant selection)
POST /api/cart/items
{
  "productId": 2,
  "quantity": 1
}
→ Checks: product (id=2) has stock >= 1
→ UnitPrice = 49.99 + 0 = 49.99
→ CartItem created
```

---

## Stock Computation

### When Stock is Calculated

| Event | Trigger | New StockQuantity |
|-------|---------|-------------------|
| Create Variant | ProductVariantsService.CreateAsync() | SUM of all variants |
| Update Variant Stock | ProductVariantsService.UpdateAsync(dto.StockQuantity) | SUM of all variants |
| Delete Variant | ProductVariantsService.DeleteAsync() | SUM of remaining, or 0 if last |
| Create Product (no variants) | ProductsService.CreateAsync() | User-provided value |

### Stock Recalculation Function

```csharp
private async Task RecalculateProductStockAsync(int productId)
{
    var totalStock = await _unitOfWork.ProductVariants
        .GetTotalStockByProductAsync(productId);
    
    var product = await _unitOfWork.Products.GetByIdAsync(productId);
    product.StockQuantity = totalStock;
    product.UpdatedAt = DateTime.UtcNow;
    await _unitOfWork.Products.UpdateAsync(product);
}
```

---

## Error Handling

### Cart Operations

| Error | Cause | Solution |
|-------|-------|----------|
| "This product has variants. You must select a variant." | HasVariants=true, no productVariantId | Add productVariantId |
| "This product does not have variants." | HasVariants=false, productVariantId provided | Remove productVariantId |
| "Insufficient stock" | Quantity > Available | Reduce quantity or check stock |
| "Product variant not found" | Invalid productVariantId | Verify variant exists |
| "Product is not available" | Product.IsActive = false | Check product status |

### Variant Operations (Admin)

| Error | Cause | Solution |
|-------|-------|----------|
| "Cannot delete variant with active cart items" | Variant in use | Remove from carts first |
| "Cannot add variants to inactive product" | Product.IsActive = false | Activate product first |
| "Variant not found for this product" | Wrong productId or variantId | Verify IDs |

---

## Database Integrity Constraints

```sql
-- Products table
ALTER TABLE Products ADD HasVariants BIT NOT NULL DEFAULT 0;
ALTER TABLE Products ADD StockQuantity INT NOT NULL DEFAULT 0;

-- CartItems table
ALTER TABLE CartItems ALTER COLUMN ProductVariantId INT NULL;
ALTER TABLE CartItems ADD ProductId INT NOT NULL;
ALTER TABLE CartItems ADD FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE;
ALTER TABLE CartItems ADD FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id) ON DELETE SET NULL;
```

---

## Performance Notes

- **Stock calculation:** O(n) where n = number of variants
- **Stock validation:** O(1) single lookup
- **Cart queries:** Includes eager loading of Product and ProductVariant
- **Pagination:** All list endpoints support page/pageSize

---

## Migration History

| Date | Migration | Change |
|------|-----------|--------|
| 2026-03-01 | RedesignStockWithHasVariantsFlag | Added HasVariants, StockQuantity to Product; made ProductVariantId nullable; added ProductId to CartItem |

---

## Version Info

- **API Version:** 1.0.0
- **Last Updated:** March 1, 2026
- **Framework:** ASP.NET Core 10.0
- **Database:** SQL Server
