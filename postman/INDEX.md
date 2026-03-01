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

3. **[API_SPECIFICATION.md](specs/API_SPECIFICATION.md)** - Technical details
   - Data models
   - Business rules
   - Error handling
   - Database structure

---

## 📦 What's Included

### Collection: `ECommerce_API.json`

**Sections:**
- 🔐 **Authentication** - Register, Login, Refresh Token, Forgot Password, Verify OTP, Reset Password
- 📦 **Products** - CRUD operations, filtering
- 🎨 **Product Variants** - Size/Color management
- 🛒 **Cart** - Shopping cart operations
- 📂 **Categories & Brands** - Master data
- 👨‍💼 **Admin Management** - Cart controls

**Total Endpoints:** 45+

### Environment: `Development.json`

**Key Variables:**
- `baseUrl` - API base (localhost:5000/api)
- `authToken` - Auto-set after login
- `userId` - Auto-set after login
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
2. Add variants → POST /api/products/{id}/variants (×3)
3. Stock auto-calculated → GET /api/products/{id}
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
- Cart operations: [README.md - Shopping Cart Logic](README.md#-shopping-cart-logic)
- Common workflows: [QUICK_START.md](QUICK_START.md)

### For Backend Developers
- API architecture: [API_SPECIFICATION.md](specs/API_SPECIFICATION.md)
- Data models: [API_SPECIFICATION.md - Data Model](specs/API_SPECIFICATION.md#data-model)
- Error handling: [API_SPECIFICATION.md - Error Handling](specs/API_SPECIFICATION.md#error-handling)

### For QA/Testers
- Test workflows: [README.md - Common Workflows](README.md#-common-workflows)
- Error scenarios: [README.md - Troubleshooting](README.md#-troubleshooting)
- All endpoints: [Collection ECommerce_API.json](collections/ECommerce_API.json)

### For Admins
- Admin endpoints: [README.md - Admin Endpoints](README.md#-admin-endpoints)
- Stock management: [API_SPECIFICATION.md - Stock Management](specs/API_SPECIFICATION.md#stock-management-system)

---

## ⚡ Common Tasks

| Task | Resource | Time |
|------|----------|------|
| Get started quickly | [QUICK_START.md](QUICK_START.md) | 5 min |
| Understand stock system | [API_SPECIFICATION.md](specs/API_SPECIFICATION.md#stock-management-system) | 10 min |
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
- ✅ `API_SPECIFICATION.md` - Technical reference

---

## 🔄 Environment Setup

### Development Environment

```json
{
  "baseUrl": "http://localhost:5000/api",
  "adminBaseUrl": "http://localhost:5000/api/admin",
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

**Date:** March 1, 2026
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
