# Orders, Coupons & Payments - Postman Collection

## 📦 What's Included

The Postman collection has been updated with complete endpoints for:

### ✅ Orders Folder (7 endpoints)
1. **Create Order** - `POST /api/orders`
2. **Get My Orders** - `GET /api/orders/my-orders`
3. **Get Order By ID** - `GET /api/orders/{orderId}`
4. **Create Payment Intent** - `POST /api/orders/{orderId}/payment`
5. **Cancel Order** - `POST /api/orders/{orderId}/cancel`
6. **Get All Orders (Admin)** - `GET /api/orders`
7. **Update Order Status (Admin)** - `PATCH /api/orders/{orderId}/status`

### ✅ Coupons Folder (12 endpoints)
1. **Get My Coupons** - `GET /api/coupons/my-coupons`
2. **Get Active Coupons** - `GET /api/coupons/active`
3. **Get Coupon By Code** - `GET /api/coupons/code/{code}`
4. **Validate Coupon** - `POST /api/coupons/validate`
5. **Get All Coupons (Admin)** - `GET /api/coupons`
6. **Create Coupon (Admin)** - `POST /api/coupons`
7. **Assign Coupon to User (Admin)** - `POST /api/coupons/{id}/assign-user/{userId}`
8. **Bulk Assign Coupon (Admin)** - `POST /api/coupons/{id}/assign-users`
9. **Remove Coupon from User (Admin)** - `DELETE /api/coupons/{id}/remove-user/{userId}`
10. **Get Coupon Users (Admin)** - `GET /api/coupons/{id}/users`
11. **Update Coupon (Admin)** - `PUT /api/coupons/{id}`
12. **Delete Coupon (Admin)** - `DELETE /api/coupons/{id}`

### ✅ Webhooks Folder (1 endpoint)
1. **Stripe Webhook** - `POST /api/webhooks/stripe`
   - For local testing with Stripe CLI

---

## 🔧 New Environment Variables

Added to `Development.json`:

```json
{
  "orderId": "1",
  "addressId": "1",
  "couponId": "1",
  "couponCode": "SAVE10"
}
```

---

## 🚀 Quick Start

### 1. Import Updated Collection
- File: `/postman/collections/ECommerce_API.json`
- Import in Postman: **Import** → **Upload Files**

### 2. Import Updated Environment
- File: `/postman/environments/Development.json`
- Set as active environment in Postman

### 3. Test Payment Flow

**Step 1:** Login to get auth token
```
POST /api/auth/login
```

**Step 2:** Create an order with coupon
```
POST /api/orders
{
  "addressId": 1,
  "orderItems": [
    {"productId": 1, "productVariantId": 1, "quantity": 2}
  ],
  "couponCode": "SAVE10"
}
```
✏️ Copy the `orderId` from response

**Step 3:** Create payment intent
```
POST /api/orders/{orderId}/payment
{
  "paymentMethodType": "card"
}
```
✏️ Copy the `clientSecret` from response

**Step 4:** Confirm payment in Stripe Dashboard
- Go to: https://dashboard.stripe.com/test/payments
- Find payment with the `paymentIntentId`
- Click "Confirm Payment"
- Webhook automatically updates order status to "Paid"

**Step 5:** Verify order status
```
GET /api/orders/{orderId}
```
✅ Status should be "Paid"

---

## 🎯 User-Coupon Access Testing

### Scenario: User can only use assigned coupons

**As Admin:**
1. Create coupon:
```
POST /api/coupons
{
  "code": "SAVE20",
  "discountType": "Percentage",
  "discountValue": 20,
  ...
}
```

2. Assign to user:
```
POST /api/coupons/1/assign-user/user-123
```

**As User:**
3. Get my coupons:
```
GET /api/coupons/my-coupons
```
✅ Only shows assigned coupons

4. Create order with coupon:
```
POST /api/orders
{
  "couponCode": "SAVE20",
  ...
}
```
✅ Works if assigned, fails if not

---

## 📋 Test Card Numbers

| Card Number         | Scenario            |
|---------------------|---------------------|
| 4242 4242 4242 4242 | Success             |
| 4000 0000 0000 0002 | Declined            |
| 4000 0025 0000 3155 | Requires 3DS        |

**Expiry:** Any future date (e.g., 12/25)  
**CVC:** Any 3 digits (e.g., 123)  
**ZIP:** Any 5 digits (e.g., 12345)

---

## 🔔 Stripe CLI Setup Required

For webhooks to work locally:

```bash
stripe login
stripe listen --forward-to https://localhost:7286/api/webhooks/stripe
```

Copy the webhook secret to `appsettings.json`:
```json
"Stripe": {
  "WebhookSecret": "whsec_test_xxxxx"
}
```

---

## 📚 Full Documentation

- **Complete Payment Testing Guide**: [STRIPE_TESTING_GUIDE.md](./STRIPE_TESTING_GUIDE.md)
- **Stripe Setup**: `../STRIPE_SETUP_GUIDE.md`
- **General API Docs**: [README.md](./README.md)

---

## ✅ Ready to Test!

1. ✅ Postman collection imported
2. ✅ Development environment configured
3. ✅ Stripe CLI running
4. ✅ Application running (`dotnet run`)
5. ✅ Start testing from Orders folder

**Happy Testing! 🚀**
