# Quick Start Guide - ECommerce API

## 5-Minute Setup

### 1️⃣ Import to Postman
- Click **Import** → Select `ECommerce_API.json`
- Select environment `Development.json`

### 2️⃣ Login & Get Token
```
POST /api/auth/login
{
  "email": "test@example.com", 
  "password": "Test@123"
}
```
✅ Token auto-set in environment
✅ Refresh token auto-stored for token renewal

**Other Auth Endpoints:**
- `POST /api/auth/register` - Create account
- `POST /api/auth/refresh-token` - Renew expired token
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/verify-otp` - Verify OTP code
- `POST /api/auth/reset-password` - Complete password reset

### 3️⃣ Test Endpoints

#### Browse Products
```
GET /api/products?page=1&pageSize=10
```

#### Add to Cart (Product WITH Variants)
```
POST /api/cart/items
{
  "quantity": 1,
  "productId": 1,          ← Required
  "productVariantId": 5    ← Required for variant products
}
```

#### Add to Cart (Product WITHOUT Variants)
```
POST /api/cart/items
{
  "quantity": 1,
  "productId": 2           ← Just product ID, no variant
}
```

#### View Cart
```
GET /api/cart
GET /api/cart/summary      ← Shows subtotal, tax, total
```

#### Manage WishList (New!)
```
POST /api/products/{id}/wishlist              ← Add to favorites
DELETE /api/products/{id}/wishlist            ← Remove from favorites
GET /api/wishlist?page=1&pageSize=10          ← View all favorites
GET /api/products/{id}/wishlist/check         ← Check if favorite
```
**Note:** Products endpoint returns `isFavorite: true/false` based on your wishlist

#### Manage Product Images (Admin)
```
POST /api/products/{id}/images (multipart)    ← Upload image (Cloudinary)
PUT /api/products/{id}/images/{imageId} (multipart) ← Replace image
DELETE /api/products/{id}/images/{imageId}    ← Delete image
GET /api/products/{id}/images                 ← List images
GET /api/products/{id}/images/{imageId}       ← Get single image
```
**Formats:** JPG, PNG, GIF, WebP (auto-deleted from Cloudinary)

---

## 🛑 Key Differences

| Feature | WITH Variants | WITHOUT Variants |
|---------|---------------|------------------|
| Example | T-Shirt (S/M/L) | Poster |
| `HasVariants` | `true` | `false` |
| Add to cart | Must include `productVariantId` | Only `productId` |
| Stock source | Variant level | Product level |

---

## 📝 Typical Flows

### Admin: Create Product with Sizes
```
1. Create product: POST /api/products/create
   { "hasVariants": true, "stockQuantity": 0 }

2. Add variants:
   POST /api/products/{{id}}/variants
   [
     { "size": "S", "color": "Blue", "stockQuantity": 50 },
     { "size": "M", "color": "Blue", "stockQuantity": 60 }
   ]

3. Auto-calculated: Product.StockQuantity = 110
```

### Customer: Shop
```
1. POST /api/auth/login → Get token
2. GET /api/products → Browse (shows isFavorite for your favorites)
3. POST /api/cart/items → Add selected variant
4. GET /api/cart/summary → View total
```

### Customer: Manage Favorites
```
1. POST /api/auth/login → Get token
2. POST /api/products/{id}/wishlist → Add to favorites
3. GET /api/wishlist → View all favorites (paginated)
4. DELETE /api/products/{id}/wishlist → Remove from favorites
5. GET /api/products/{id}/wishlist/check → Check if product is favorite
```

---

## 🔗 All Endpoints at a Glance

**Products:** `GET, POST, PUT, DELETE /api/products`
**Variants:** `GET, POST, PUT, DELETE /api/products/{id}/variants`
**Cart:** `GET, POST, PUT, DELETE /api/cart, /api/cart/items`
**WishList:** `GET, POST, DELETE /api/wishlist, /api/products/{id}/wishlist`
**Admin:** `GET, DELETE /api/admin/carts, /api/admin/cart-items`

---

## ⚠️ Common Mistakes

❌ **Error:** "This product has variants. You must select a variant."
✅ **Fix:** Add `productVariantId` to request

❌ **Error:** "This product does not have variants."
✅ **Fix:** Remove `productVariantId` from request

❌ **Error:** "Unauthorized"
✅ **Fix:** Login first, check `authToken` in environment

---

## 🎯 Stock Logic

**Products WITH Variants:**
```
Product.StockQuantity = SUM of all variant stocks
(Auto-calculated, don't manually set)
```

**Products WITHOUT Variants:**
```
Product.StockQuantity = Manual value
```

---

## 📊 Response Examples

### Success
```json
{
  "status": true,
  "message": "Product added to cart successfully.",
  "data": { "id": 1, "quantity": 2, ... }
}
```

### Error
```json
{
  "status": false,
  "message": "Insufficient stock. Available: 5, Requested: 10",
  "data": null
}
```

---

## 🚀 Next Steps

1. ✅ Import collection
2. ✅ Login and test
3. ✅ Try both product types
4. ✅ Test cart operations
5. ✅ Check admin endpoints

---

**Full docs:** See `README.md`
**Need help?** Check troubleshooting section
