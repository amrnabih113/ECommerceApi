# ECommerce API - Complete Endpoint Documentation

**Base URL**: `https://localhost:7286/api`
**Version**: v1

---

## Table of Contents
1. [Authentication](#authentication)
2. [Products](#products)
3. [Categories](#categories)
4. [Brands](#brands)
5. [Shopping Cart](#shopping-cart)
6. [Cart Items](#cart-items)
7. [Orders](#orders)
8. [Coupons](#coupons)
9. [Banners](#banners)
10. [Reviews](#reviews)
11. [Wishlist](#wishlist)
12. [Addresses](#addresses)
13. [Webhooks](#webhooks)
14. [Rate Limiting](#rate-limiting)

---

## Authentication

### POST `/auth/register`
**Auth Required**: No
**Rate Limit**: 5 req/min (auth policy)
Create a new user account

```json
{
  "userName": "newuser",
  "email": "user@example.com",
  "password": "Password@123"
}
```

### POST `/auth/login`
**Auth Required**: No
**Rate Limit**: 5 req/min (auth policy)
Login and get JWT token

```json
{
  "email": "user@example.com",
  "password": "Password@123"
}
```

### POST `/auth/refresh-token`
**Auth Required**: No
Refresh JWT access token

```json
{
  "refreshToken": "refresh-token-value"
}
```

### POST `/auth/verify-otp`
**Auth Required**: No
Verify OTP during password reset

```json
{
  "email": "user@example.com",
  "otp": "123456"
}
```

### POST `/auth/resend-otp`
**Auth Required**: No
Resend OTP for password reset

```json
{
  "email": "user@example.com"
}
```

### POST `/auth/reset-password`
**Auth Required**: No
Reset password with OTP

```json
{
  "email": "user@example.com",
  "otp": "123456",
  "newPassword": "NewPassword@123"
}
```

---

## Products

### GET `/products`
**Auth Required**: Yes (authenticated users)
**Rate Limit**: 100 req/min (general)
Get all products with pagination and filtering

**Query Parameters**:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)
- `brandId`: Filter by brand ID
- `categoryId`: Filter by category ID
- `minPrice`: Filter by minimum price
- `maxPrice`: Filter by maximum price
- `sortBy`: Sort by (name, price, createdAt)
- `sortOrder`: asc or desc

### GET `/products/{id}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get product details by ID

### GET `/products/sales`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get products on sale (with discounts)

**Query Parameters**: Same as GET `/products`

### GET `/products/best-sales`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get best-selling products

**Query Parameters**: 
- `page`: Page number
- `pageSize`: Items per page
- `categoryId`: Filter by category
- `brandId`: Filter by brand

### GET `/products/search`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Search products by term

**Query Parameters**:
- `term`: Search term (required)
- `page`: Page number
- `pageSize`: Items per page

### GET `/products/search/recommendations`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get search recommendations by term

**Query Parameters**:
- `term`: Search term (required)
- `size`: Number of recommendations (default: 5)

### GET `/products/brand/{brandId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get products by brand

**Query Parameters**: Same as GET `/products`

### GET `/products/category/{categoryId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get products by category

**Query Parameters**: Same as GET `/products`

### POST `/products/create`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Create new product

```json
{
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.99,
  "categoryId": 1,
  "brandId": 1,
  "hasVariants": false,
  "stockQuantity": 100,
  "hasDiscount": false,
  "priceAfterDiscount": 0
}
```

### PUT `/products/{id}`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Update product

### DELETE `/products/{id}`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Delete product

---

## Categories

### GET `/categories`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get all categories with pagination

**Query Parameters**:
- `page`: Page number
- `pageSize`: Items per page
- `parentCategoryId`: Filter by parent category

### GET `/categories/{id}`
**Auth Required**: Yes
Get category by ID

### POST `/categories/create`
**Auth Required**: Yes (Admin only)
Create new category

### PUT `/categories/{id}`
**Auth Required**: Yes (Admin only)
Update category

### DELETE `/categories/{id}`
**Auth Required**: Yes (Admin only)
Delete category

---

## Brands

### GET `/brands`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get all brands with pagination

### GET `/brands/{id}`
**Auth Required**: Yes
Get brand by ID

### POST `/brands/create`
**Auth Required**: Yes (Admin only)
Create new brand

### PUT `/brands/{id}`
**Auth Required**: Yes (Admin only)
Update brand

### DELETE `/brands/{id}`
**Auth Required**: Yes (Admin only)
Delete brand

---

## Shopping Cart

### GET `/carts`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get current user cart

### POST `/carts/add`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Add item to cart

```json
{
  "productId": 1,
  "quantity": 1,
  "productVariantId": null
}
```

### PUT `/carts/update/{cartItemId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Update cart item quantity

```json
{
  "quantity": 2
}
```

### DELETE `/carts/clear`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Clear cart

### DELETE `/carts/remove/{cartItemId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Remove item from cart

---

## Cart Items

### GET `/cart-items`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get cart items

### POST `/cart-items/add`
**Auth Required**: Yes
Add cart item

### DELETE `/cart-items/{id}`
**Auth Required**: Yes
Delete cart item

---

## Orders

### POST `/orders`
**Auth Required**: Yes (User/Admin)
**Rate Limit**: 20 req/min (orders policy)
Create new order

```json
{
  "items": [
    {
      "productId": 1,
      "productVariantId": null,
      "quantity": 1
    }
  ],
  "shippingAddressId": 1,
  "couponCode": null
}
```

### GET `/orders`
**Auth Required**: Yes
**Rate Limit**: 20 req/min
Get all orders (Admin) or user's orders

**Query Parameters**:
- `page`: Page number
- `pageSize`: Items per page
- `status`: Filter by status (Pending, Paid, Shipped, Delivered, Cancelled)

### GET `/orders/{orderId}`
**Auth Required**: Yes
Get order details

### POST `/orders/{orderId}/payment`
**Auth Required**: Yes
**Rate Limit**: 20 req/min
Create payment intent for order

### POST `/orders/{orderId}/cancel`
**Auth Required**: Yes
**Rate Limit**: 20 req/min
Cancel order

### PUT `/orders/{orderId}/status`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 20 req/min
Update order status

```json
{
  "status": "Shipped"
}
```

---

## Coupons

### POST `/coupons/create`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Create new coupon

```json
{
  "code": "SUMMER50",
  "discountType": "Percentage",
  "discountValue": 50,
  "expirationDate": "2026-12-31",
  "maxUses": 100,
  "minPurchaseAmount": 50
}
```

### GET `/coupons`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get all coupons

### GET `/coupons/{couponCode}/validate`
**Auth Required**: Yes
Validate coupon code

---

## Banners

### GET `/banners`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Get all banners with pagination

**Query Parameters**:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 10)

### GET `/banners/active`
**Auth Required**: No
**Rate Limit**: 100 req/min
Get active banners (for homepage display)

### GET `/banners/{id}`
**Auth Required**: No
Get banner by ID

### POST `/banners`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Create new banner

```json
{
  "imageUrl": "https://example.com/banner.jpg",
  "isActive": true,
  "title": "Banner Title",
  "description": "Banner Description",
  "link": "https://example.com/link",
  "displayOrder": 0
}
```

### PUT `/banners/{id}`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Update banner (all fields optional)

```json
{
  "imageUrl": "https://example.com/new-banner.jpg",
  "isActive": true,
  "title": "Updated Title",
  "displayOrder": 1
}
```

### DELETE `/banners/{id}`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Delete banner

### PATCH `/banners/{id}/toggle`
**Auth Required**: Yes (Admin only)
**Rate Limit**: 100 req/min
Toggle banner active status

---

## Reviews

### GET `/reviews/product/{productId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get product reviews

### POST `/reviews/add`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Add product review

```json
{
  "productId": 1,
  "rating": 5,
  "comment": "Great product!"
}
```

### DELETE `/reviews/{reviewId}`
**Auth Required**: Yes
Delete your review

---

## Wishlist

### GET `/wishlist`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get user wishlist

### POST `/wishlist/add/{productId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Add product to wishlist

### DELETE `/wishlist/remove/{productId}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Remove product from wishlist

### DELETE `/wishlist/clear`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Clear wishlist

---

## Addresses

### GET `/addresses`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Get user addresses

### POST `/addresses`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Create new address

```json
{
  "fullName": "John Doe",
  "phoneNumber": "+1234567890",
  "street": "123 Main St",
  "city": "City Name",
  "state": "State",
  "postalCode": "12345",
  "country": "Country",
  "isDefault": true
}
```

### PUT `/addresses/{id}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Update address

### DELETE `/addresses/{id}`
**Auth Required**: Yes
**Rate Limit**: 100 req/min
Delete address

---

## Webhooks

### POST `/webhooks/stripe`
**Auth Required**: No
**Rate Limit**: None (Stripe signature verified)
Receive Stripe webhook events

**Headers**:
- `Stripe-Signature`: Webhook signature from Stripe

**Supported Events**:
- `payment_intent.succeeded`
- `payment_intent.payment_failed`
- `charge.refunded`

---

## Rate Limiting

### Policy Details

| Policy | Limit | Window |
|--------|-------|--------|
| auth | 5 req/min | 1 minute |
| orders | 20 req/min | 1 minute |
| general | 100 req/min | 1 minute |

**Rate Limit Headers**:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 99
X-RateLimit-Reset: 1614556800
```

**Rate Limit Exceeded Response** (429):
```json
{
  "title": "Too Many Requests",
  "status": 429,
  "detail": "The request quota per minute has been exceeded. Try again in a few moments."
}
```

---

## Authentication Headers

All authenticated endpoints require:
```
Authorization: Bearer <JWT_TOKEN>
```

---

## Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation successful",
  "data": {}
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error message",
  "errors": {}
}
```

### Paginated Response
```json
{
  "success": true,
  "message": "Data fetched",
  "data": {
    "items": [],
    "totalItems": 100,
    "page": 1,
    "pageSize": 10
  }
}
```

---

## Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 429 | Too Many Requests (Rate Limited) |
| 500 | Internal Server Error |

---

## Environment Variables

For Postman, set these environment variables:

- `baseUrl`: https://localhost:7286/api
- `token`: JWT token from login response
- `refreshToken`: Refresh token from login response
- `productId`: ID of a product for testing
- `categoryId`: ID of a category for testing
- `orderId`: ID of an order for testing
- `bannerId`: ID of a banner for testing

---

**Last Updated**: March 5, 2026
**API Version**: v1.0.0
