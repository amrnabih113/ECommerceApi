# 📬 Postman Collection Setup Guide

## Overview
This folder contains the complete Postman collection and environment for the ECommerce API. All 54+ endpoints are organized by feature with proper request/response examples.

## 📦 Collection & Environment Files

### `collections/ECommerce_API.json`
- Complete API collection with all endpoints
- Organized by feature (Auth, Products, Images, Cart, WishList, etc.)
- Pre-configured with request bodies and response examples
- Supports both authentication and admin endpoints

### `environments/Development.json`
- Development environment with default variable values
- Pre-configured for `localhost:7286`
- All variable placeholders ready to fill

## 🚀 Quick Start

### 1. Import Collection
1. Open **Postman**
2. Click **Import** (top-left)
3. Select **File** tab
4. Choose `collections/ECommerce_API.json`
5. Click **Import**

### 2. Import Environment
1. Go to **Environments** (left sidebar)
2. Click **Import**
3. Choose `environments/Development.json`
4. Click **Import**
5. Select **Development** environment from dropdown (top-right)

### 3. Set Authentication
After login, update these variables:
- **authToken**: Paste the JWT token from login response
- **userId**: Paste the user ID from login response
- **refreshToken**: Paste the refresh token for token refresh endpoint

## 📋 Collection Structure

```
ECommerce API
├── 🔐 Authentication (6 endpoints)
│   ├── Register
│   ├── Login
│   ├── Refresh Token
│   ├── Forgot Password
│   ├── Verify OTP
│   └── Reset Password
│
├── 📦 Products (7 endpoints)
│   ├── Get All Products
│   ├── Get Product by ID
│   ├── Create Product
│   ├── Update Product
│   ├── Delete Product
│   ├── Get by Category
│   └── Get by Brand
│
├── 🖼️ Product Images (5 endpoints)
│   ├── List Product Images
│   ├── Get Single Image
│   ├── Upload Image
│   ├── Update Image
│   └── Delete Image
│
├── 🎨 Product Variants (4 endpoints)
│   ├── Get Variants
│   ├── Create Variants
│   ├── Update Variant
│   └── Delete Variant
│
├── ⭐ Reviews (4 endpoints)
│   ├── Get Product Reviews
│   ├── Create Review
│   ├── Update Review
│   └── Delete Review
│
├── ❤️ WishList (4 endpoints)
│   ├── Get My WishList
│   ├── Add to WishList
│   ├── Remove from WishList
│   └── Check if in WishList
│
├── 🛒 Cart (7 endpoints)
│   ├── Get Cart Summary
│   ├── Get Cart Items
│   ├── Add to Cart (with Variants)
│   ├── Add to Cart (without Variants)
│   ├── Update Cart Item
│   ├── Remove from Cart
│   └── Clear Cart
│
├── 📂 Categories (4 endpoints)
│   ├── Get All Categories
│   ├── Create Category
│   ├── Update Category
│   └── Delete Category
│
├── 🏷️ Brands (4 endpoints)
│   ├── Get All Brands
│   ├── Create Brand
│   ├── Update Brand
│   └── Delete Brand
│
└── 👨‍💼 Admin Cart Management (5 endpoints)
    ├── Get All Carts
    ├── Get Specific Cart
    ├── Get User's Cart
    ├── Delete Cart
    └── Delete Cart Item
```

## 🔑 Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `baseUrl` | http://localhost:7286/api | Main API base URL |
| `adminBaseUrl` | http://localhost:7286/api/admin | Admin endpoints base URL |
| `authToken` | (empty) | JWT authentication token |
| `refreshToken` | (empty) | Refresh token for token renewal |
| `userId` | (empty) | Current user ID |
| `productId` | 1 | Sample product ID |
| `imageId` | 1 | Sample image ID |
| `variantId` | 1 | Sample variant ID |
| `categoryId` | 1 | Sample category ID |
| `brandId` | 1 | Sample brand ID |
| `cartId` | 1 | Sample cart ID |
| `cartItemId` | 1 | Sample cart item ID |
| `reviewId` | 1 | Sample review ID |
| `page` | 1 | Pagination page number |
| `pageSize` | 10 | Items per page |

## 🚦 Workflow Examples

### Complete User Journey
1. **Register** - Create new account
2. **Login** - Get authentication token
3. **Browse Products** - View all or filtered products (isFavorite visible)
4. **Add to WishList** - Save favorite items
5. **Add to Cart** - Add items with variants
6. **View Cart** - Review selected items
7. **Leave Review** - Rate purchased products

### Admin Product Setup
1. **Create Category** - Set up product categories
2. **Create Brand** - Add brand information
3. **Create Product** - Add new product with variants
4. **Upload Images** - Add product images (Cloudinary)
5. **View Products** - Verify setup complete

## 💡 Tips & Tricks

### Automatic Token Management
- After **Login** or **Refresh Token**, save the token to the `authToken` variable
- Use **Postman Scripts** to automate this (see collection Tests tab)

### Image Upload
- Images use **multipart/form-data**
- Currently integrated with **Cloudinary**
- `isMain` flag sets primary product image

### Product Variants
- Only required if `hasVariants = true`
- Each variant tracks `size`, `color`, `stockQuantity`
- Cart item requires either product-only OR product + variant

### Pagination
- All list endpoints support `page` and `pageSize` parameters
- Default: page=1, pageSize=10
- Modify in Environment or request URL

### Admin Endpoints
- Require authentication
- Use separate `adminBaseUrl` variable
- Limited to admin users only

## 🐛 Common Issues

### "Token Expired"
- Run **Refresh Token** endpoint with current `refreshToken`
- Update `authToken` variable with new token

### "Port not accessible"
- Ensure API is running on port 7286
- Check `appsettings.json` for correct port configuration

### "Unauthorized" on Admin Endpoints
- Confirm you're logged in as admin user
- Verify `authToken` is properly set

## 📚 Additional Resources

- [README.md](README.md) - Detailed endpoint documentation
- [INDEX.md](INDEX.md) - Collection index and workflows
- [QUICK_START.md](QUICK_START.md) - Quick reference guide

## 🔗 Related Documentation

- See [SWAGGER_README.md](../SWAGGER_README.md) for Swagger/OpenAPI documentation
- Check [swagger.json](../swagger.json) for API schema
