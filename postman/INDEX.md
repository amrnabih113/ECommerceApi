# ECommerce API - Postman Documentation Index

Welcome to the complete Postman documentation for the ECommerce API! 📚

## 📁 Documentation Structure

```
postman/
├── collections/
│   └── ECommerce_API.json           ← Main API collection
├── environments/
│   └── Development.json             ← Environment variables
├── globals/
│   └── Global_Variables.json        ← Global constants
├── specs/
│   └── API_SPECIFICATION.md         ← Detailed API spec
├── README.md                        ← Full documentation
├── QUICK_START.md                   ← 5-minute setup
├── STRIPE_TESTING_GUIDE.md          ← Payment testing workflow
├── ORDERS_COUPONS_ENDPOINTS.md      ← Orders & Coupons quick reference
└── INDEX.md                         ← This file
```

---

## 🚀 Getting Started

### New to the API? Start here:

1. **[QUICK_START.md](QUICK_START.md)** - 5-minute setup
   - Import collection
   - Login & get token
   - Test basic endpoints
   - Common mistakes

2. **[README.md](README.md)** - Complete reference
   - Detailed workflows
   - All endpoints
   - Response formats
   - Troubleshooting

3. **[STRIPE_TESTING_GUIDE.md](STRIPE_TESTING_GUIDE.md)** - Payment testing
   - Complete local payment flow
   - Stripe CLI setup
   - Test card numbers
   - Coupon testing

4. **[ORDERS_COUPONS_ENDPOINTS.md](ORDERS_COUPONS_ENDPOINTS.md)** - Quick reference
   - All Orders endpoints
   - All Coupons endpoints
   - New environment variables

5. **[API_SPECIFICATION.md](specs/API_SPECIFICATION.md)** - Technical details
   - Data models
   - Business rules
   - Error handling
   - Database structure

---

## 📦 What's Included

### Collection: `ECommerce_API.json`

**Sections:**
- 🔐 **Authentication** - Register, Login, Refresh Token, Forgot Password, Verify OTP, Reset Password
- 📦 **Products** - CRUD operations, filtering, **Search & Recommendations**
- 🔍 **Search** - Products, Brands, Categories with autocomplete recommendations
- 🖼️ **Product Images** - Upload, update, delete (Cloudinary CDN)
- 🎨 **Product Variants** - Size/Color management
- 🛒 **Cart** - Shopping cart operations
- ❤️ **WishList** - Favorites, add/remove, check favorites
- 📂 **Categories & Brands** - Master data with search
- ⭐ **Reviews** - Product ratings and comments
- �️ **Orders** - Create, track, payment intents, cancel orders
- 💳 **Coupons** - User coupons, admin assignment, bulk operations
- 🔔 **Webhooks** - Stripe payment webhooks
- 👨‍💼 **Admin Management** - Cart controls, order status updates

**Total Endpoints:** 80+ (including Orders, Payments, Coupons management)

### Environment: `Development.json`

**Key Variables:**
- `baseUrl` - API base (localhost:7286/api)
- `authToken` - Auto-set after login
- `userId` - Auto-set after login
- `orderId` - Order ID for testing
- `addressId` - Address ID for orders
- `couponId` - Coupon ID for admin operations
- `couponCode` - Coupon code to test
- Test IDs for quick testing

### Globals: `Global_Variables.json`

**Common Values:**
- Test credentials
- Pagination defaults
- Test product/variant IDs

---

## 🔄 Popular Workflows

### 1️⃣ Create Product with Variants (Admin)
```
1. Create product → POST /api/products/create
2. Upload images → POST /api/products/{id}/images (×2-3)
3. Add variants → POST /api/products/{id}/variants (×3)
4. Stock auto-calculated → GET /api/products/{id}
```
See: [README.md - Product Management](README.md#-product-management)

### 2️⃣ Customer Shopping
```
1. Login → POST /api/auth/login
2. Browse → GET /api/products
3. Add to cart → POST /api/cart/items
4. View cart → GET /api/cart/summary
```
See: [QUICK_START.md - Customer Flow](QUICK_START.md#customer-shop)

### 3️⃣ Admin Stock Management
```
1. Update variant stock → PUT /api/products/{id}/variants/{variantId}
2. Product stock auto-recalculated
3. View all carts → GET /api/admin/carts
```
See: [README.md - Admin Endpoints](README.md#-admin-endpoints)

### 4️⃣ Manage WishList (Favorites)
```
1. Login → POST /api/auth/login
2. Browse products → GET /api/products (returns isFavorite status)
3. Add to wishlist → POST /api/products/{id}/wishlist
4. View my favorites → GET /api/wishlist
5. Remove from wishlist → DELETE /api/products/{id}/wishlist
6. Check if favorite → GET /api/products/{id}/wishlist/check
```
See: [README.md - WishList Management](README.md#-wishlist-management)

### 5️⃣ Product Search with Autocomplete
```
1. Get search suggestions → GET /api/products/search/recommendations?term=shi&size=5
2. Full search → GET /api/products/search?term=shirt&page=1&pageSize=10
3. Same for brands → GET /api/brands/search & GET /api/brands/search/recommendations
4. Same for categories → GET /api/categories/search & GET /api/categories/search/recommendations
```
See: [README.md - Search & Recommendations](README.md#-search--recommendations)

### 6️⃣ Complete Order & Payment (Stripe)
```
1. Login → POST /api/auth/login
2. Create order → POST /api/orders
3. Create payment → POST /api/orders/{id}/payment
4. Confirm in Stripe Dashboard → Webhook fires → Order status: Paid
5. Verify order → GET /api/orders/{id}
```
See: [STRIPE_TESTING_GUIDE.md](STRIPE_TESTING_GUIDE.md)

### 7️⃣ Admin Coupon Management
```
1. Create coupon → POST /api/coupons (admin)
2. Assign to user → POST /api/coupons/{id}/assign-user/{userId}
3. User views → GET /api/coupons/my-coupons
4. User applies → POST /api/orders (with couponCode)
```
See: [ORDERS_COUPONS_ENDPOINTS.md](ORDERS_COUPONS_ENDPOINTS.md)

---

## 🛑 Key Business Rules

### Two Types of Products

**WITH Variants** (e.g., T-Shirt with sizes)
```
HasVariants = true
→ Must select variant when adding to cart
→ Stock managed at variant level
→ Product stock = SUM of variant stocks (auto-calculated)
```

**WITHOUT Variants** (e.g., Poster)
```
HasVariants = false
→ Add directly to cart
→ Stock managed at product level
```

See: [API Specification - Business Rules](specs/API_SPECIFICATION.md#business-rules)

---

## 📊 API Response Format

All endpoints follow this structure:

### Success (200 OK)
```json
{
  "status": true,
  "message": "Operation successful",
  "data": { /* response data */ }
}
```

### Error (4xx/5xx)
```json
{
  "status": false,
  "message": "Error description",
  "data": null
}
```

See: [README.md - Response Format](README.md#-api-response-format)

---

## 🔗 Quick Links by Role

### For Frontend Developers
- Product browsing: [README.md - Product Management](README.md#-product-management)
- **Search & Autocomplete**: [README.md - Search & Recommendations](README.md#-search--recommendations)
- Cart operations: [README.md - Shopping Cart Logic](README.md#-shopping-cart-logic)
- **Orders & Payments**: [STRIPE_TESTING_GUIDE.md](STRIPE_TESTING_GUIDE.md)
- Common workflows: [QUICK_START.md](QUICK_START.md)

### For Backend Developers
- API architecture: [API_SPECIFICATION.md](specs/API_SPECIFICATION.md)
- Data models: [API_SPECIFICATION.md - Data Model](specs/API_SPECIFICATION.md#data-model)
- **Payment Integration**: [STRIPE_TESTING_GUIDE.md](STRIPE_TESTING_GUIDE.md)
- Error handling: [API_SPECIFICATION.md - Error Handling](specs/API_SPECIFICATION.md#error-handling)

### For QA/Testers
- Test workflows: [README.md - Common Workflows](README.md#-common-workflows)
- **Payment Testing**: [STRIPE_TESTING_GUIDE.md](STRIPE_TESTING_GUIDE.md)
- **Coupon Testing**: [ORDERS_COUPONS_ENDPOINTS.md](ORDERS_COUPONS_ENDPOINTS.md)
- Error scenarios: [README.md - Troubleshooting](README.md#-troubleshooting)
- All endpoints: [Collection ECommerce_API.json](collections/ECommerce_API.json)

### For Admins
- Admin endpoints: [README.md - Admin Endpoints](README.md#-admin-endpoints)
- **Coupon Management**: [ORDERS_COUPONS_ENDPOINTS.md](ORDERS_COUPONS_ENDPOINTS.md)
- Stock management: [API_SPECIFICATION.md - Stock Management](specs/API_SPECIFICATION.md#stock-management-system)

---

## ⚡ Common Tasks

| Task | Resource | Time |
|------|----------|------|
| Get started quickly | [QUICK_START.md](QUICK_START.md) | 5 min |
| **Test payment flow** | [STRIPE_TESTING_GUIDE.md](STRIPE_TESTING_GUIDE.md) | 10 min |
| **Manage coupons** | [ORDERS_COUPONS_ENDPOINTS.md](ORDERS_COUPONS_ENDPOINTS.md) | 5 min |
| Understand stock system | [API_SPECIFICATION.md](specs/API_SPECIFICATION.md#stock-management-system) | 10 min |
| Implement search/autocomplete | [README.md - Search & Recommendations](README.md#-search--recommendations) | 5 min |
| Find specific endpoint | [README.md](README.md) | 2 min |
| Test cart flow | Collection → Cart section | 5 min |
| Debug error | [README.md - Troubleshooting](README.md#-troubleshooting) | 3 min |
| Setup environment | [README.md - Getting Started](README.md#-getting-started) | 2 min |

---

## 📞 Support Resources

### Documentation Files
- **QUICK_START.md** - Fast, practical guide
- **README.md** - Comprehensive reference
- **API_SPECIFICATION.md** - Technical details
- **This file** - Navigation & overview

### Collection Features
- 40+ pre-built requests
- Auto-token capture from login
- Environment variables for easy switching
- Test scripts for validation

### Getting Help
1. Check [Troubleshooting](README.md#-troubleshooting) section
2. Review relevant workflow in [Common Workflows](README.md#-common-workflows)
3. Check API specification for business rules
4. Look for similar endpoint in collection

---

## 📋 File Checklist

Before using the API, ensure you have:

- ✅ `ECommerce_API.json` - Main collection
- ✅ `Development.json` - Development environment
- ✅ `Global_Variables.json` - Test constants
- ✅ `README.md` - Full documentation
- ✅ `QUICK_START.md` - Setup guide
- ✅ `STRIPE_TESTING_GUIDE.md` - Payment testing workflow
- ✅ `ORDERS_COUPONS_ENDPOINTS.md` - Orders & Coupons reference
- ✅ `API_SPECIFICATION.md` - Technical reference

---

## 🔄 Environment Setup

### Development Environment

```json
{
  "baseUrl": "http://localhost:7286/api",
  "adminBaseUrl": "http://localhost:7286/api/admin",
  "authToken": "Bearer ...",      // Auto-filled
  "userId": "...",                // Auto-filled
  "page": "1",
  "pageSize": "10"
}
```

### Variables Auto-Set After Login

- `authToken` ← Used for all authenticated requests
- `userId` ← Your user ID

---

## 🎯 Next Steps

1. **Import collection** → Collections/ECommerce_API.json
2. **Set environment** → Environments/Development.json
3. **Read QUICK_START** → [QUICK_START.md](QUICK_START.md)
4. **Login** → POST /api/auth/login
5. **Test endpoints** → Start with Products, then Cart

---

## 📝 Last Updated

**Date:** March 4, 2026
**API Version:** 1.0.0
**Framework:** ASP.NET Core 10.0

---

## 📚 Document Hierarchy

```
BEGIN HERE
    ↓
QUICK_START.md (5 min overview)
    ↓
README.md (move to specific section)
    ├→ Product Management?
    ├→ Shopping Cart?
    ├→ Admin Endpoints?
    └→ Troubleshooting?
    ↓
API_SPECIFICATION.md (technical deep dive)
    ├→ Data Models
    ├→ Business Rules
    ├→ Stock System
    └→ Database Schema
    ↓
Collection (hands-on testing)
```

---

**Happy coding! 🚀**
