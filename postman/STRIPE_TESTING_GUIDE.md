# Stripe & Orders Testing Guide - Postman

Complete guide to test the Stripe payment integration and order management system locally using Postman and Stripe CLI.

---

## 🔄 Prerequisites

✅ Stripe account with test keys (pk_test_*, sk_test_*)
✅ Stripe CLI installed and running (`stripe listen --forward-to https://localhost:7286/api/webhooks/stripe`)
✅ Application running on `https://localhost:7286`
✅ Postman collection imported with variables set

---

## 📋 Environment Setup in Postman

Before testing, configure these variables in your **Development** environment:

```json
{
  "baseUrl": "https://localhost:7286/api",
  "authToken": "{{auto-set after login}}",
  "userId": "{{auto-set after login}}",
  "orderId": "{{set after creating order}}",
  "addressId": "1",
  "productId": "1",
  "couponCode": "SAVE10"
}
```

---

---

## 🧪 Test Users Available

Pre-seeded test accounts with addresses:

**Admin:**
- Email: `admin@ecommerce.com`
- Password: `Admin@123`

**Customers (choose any):**
- `customer@example.com` / `Customer@123` (John Doe - 3 addresses)
- `jane@example.com` / `JaneSmith@123` (Jane Smith - 2 addresses)
- `mike@example.com` / `MikeJohnson@123` (Mike Johnson - 3 addresses)
- `sarah@example.com` / `SarahWilliams@123` (Sarah Williams - 2 addresses)

**See:** [TEST_USERS_AND_ADDRESSES.md](../TEST_USERS_AND_ADDRESSES.md) for all details

---

## 🧪 Complete Local Testing Flow

### Step 1️⃣: Authentication (Get Auth Token)

**Request:** `POST /api/auth/login`

```http
POST https://localhost:7286/api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Test@123"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "user-123",
      "userName": "testuser",
      "email": "test@example.com"
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh-token-here"
  }
}
```

**Action:** Copy `token` → Set in Postman as `{{authToken}}` environment variable

---

### Step 2️⃣: Create or Get Your Address (Required for Orders)

**Option A: Create New Address**

**Request:** `POST /api/addresses`

```http
POST https://localhost:7286/api/addresses
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "country": "USA",
  "city": "New York",
  "street": "123 Main St, Apt 4B",
  "postalCode": "10001"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Address created successfully.",
  "data": {
    "id": 1,
    "country": "USA",
    "city": "New York",
    "street": "123 Main St, Apt 4B",
    "postalCode": "10001"
  }
}
```

**Option B: Get Existing Addresses**

**Request:** `GET /api/addresses`

```http
GET https://localhost:7286/api/addresses
Authorization: Bearer {{authToken}}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "country": "USA",
      "city": "New York",
      "street": "123 Main St, Apt 4B",
      "postalCode": "10001"
    }
  ]
}
```

**Action:** Copy address `id` → Set as `{{addressId}}` (e.g., 1)

---

### Step 3️⃣: Get Product ID

**Request:** `GET /api/products?limit=5`

```http
GET https://localhost:7286/api/products?limit=5
Authorization: Bearer {{authToken}}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Laptop",
        "description": "High-performance laptop",
        "price": 999.99,
        "stock": 50
      }
    ]
  }
}
```

**Action:** Copy product `id` → Use as `{{productId}}`

---

### Step 4️⃣: Create an Order

**Request:** `POST /api/orders`

```http
POST https://localhost:7286/api/orders
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "addressId": {{addressId}},
  "orderItems": [
    {
      "productId": {{productId}},
      "productVariantId": 1,
      "quantity": 1
    }
  ],
  "couponCode": "SAVE10"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 101,
    "userId": "user-123",
    "orderNumber": "ORD-2026-001",
    "totalAmount": 899.99,
    "discountAmount": 100.00,
    "status": "Pending",
    "paymentStatus": "Pending",
    "createdAt": "2026-03-04T10:30:00"
  }
}
```

**Action:** Copy order `id` → Use as `{{orderId}}` (e.g., 101)

---

### Step 5️⃣: Create Payment Intent

**Request:** `POST /api/orders/{{orderId}}/payment`

```http
POST https://localhost:7286/api/orders/101/payment
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "paymentMethodType": "card"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentIntentId": "pi_test_abc123xyz",
    "clientSecret": "pi_test_abc123xyz_secret_xyz789",
    "amount": 89999,
    "currency": "usd",
    "status": "requires_payment_method"
  }
}
```

**Action:** Copy `clientSecret` → Use in Stripe Dashboard confirmation

---

### Step 6️⃣: Confirm Payment in Stripe Dashboard

⚠️ **This is where Stripe CLI comes in - you do NOT use your frontend**

1. **Open Stripe Dashboard**: [https://dashboard.stripe.com/test/payments](https://dashboard.stripe.com/test/payments)

2. **Find Your Payment Intent**:
   - Filter by **PaymentIntent ID**: `pi_test_abc123xyz`
   - Status shows: **Processing**

3. **Click on the payment** to open details

4. **Click "Confirm Payment"** button
   - Stripe will immediately send webhook to `stripe listen`
   - Your Stripe CLI shows: `payment_intent.succeeded`
   - Webhook forwards to `https://localhost:7286/api/webhooks/stripe`

---

### Step 7️⃣: Verify Order Status Changed to "Paid"

**Request:** `GET /api/orders/{{orderId}}`

```http
GET https://localhost:7286/api/orders/101
Authorization: Bearer {{authToken}}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 101,
    "orderNumber": "ORD-2026-001",
    "status": "Paid",
    "paymentStatus": "Succeeded",
    "updatedAt": "2026-03-04T10:35:00"
  }
}
```

✅ **Order status automatically changed from "Pending" → "Paid"** via webhook!

---

## 🧪 Test Scenarios

### Scenario 1: Successful Payment ✅

```
Test Card: 4242 4242 4242 4242
Expiry: 12/25
CVC: 123
ZIP: 12345
```

**Expected Flow:**
1. Create order → Status: `Pending`
2. Create payment → Status: `requires_payment_method`
3. Confirm in Dashboard → Webhook fires → Order status: `Paid` ✅

---

### Scenario 2: Payment Declined ❌

```
Test Card: 4000 0000 0000 0002
Expiry: 12/25
CVC: 123
ZIP: 12345
```

**Expected Flow:**
1. Create order → Status: `Pending`
2. Create payment → Status: `requires_payment_method`
3. Confirm in Dashboard → Stripe rejects
4. Webhook fires with `payment_intent.payment_failed`
5. Order remains `Pending` ❌

---

### Scenario 3: 3D Secure Payment 🔐

```
Test Card: 4000 0025 0000 3155
Expiry: 12/25
CVC: 123
ZIP: 12345
```

**Expected Flow:**
1. Create payment → Status: `requires_action`
2. Frontend shows 3DS challenge
3. User completes authentication
4. Webhook fires with `payment_intent.succeeded`
5. Order status: `Paid` ✅

---

## 🎯 Coupon Testing

### Get User's Available Coupons

**Request:** `GET /api/coupons/my-coupons`

```http
GET https://localhost:7286/api/coupons/my-coupons?page=1&limit=10
Authorization: Bearer {{authToken}}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "code": "SAVE10",
        "description": "10% off all items",
        "discountType": "Percentage",
        "discountValue": 10,
        "validFrom": "2026-01-01",
        "validUntil": "2026-12-31",
        "usageLimit": 100,
        "usedCount": 25,
        "canUse": true,
        "userUsageCount": 0
      }
    ],
    "total": 15
  }
}
```

---

### Apply Coupon to Order

Include `couponCode` in create order request:

```json
{
  "addressId": 1,
  "orderItems": [{"productId": 1, "productVariantId": 1, "quantity": 1}],
  "couponCode": "SAVE10"
}
```

**Validation:**
- ✅ User must be assigned to coupon
- ✅ Coupon must be active
- ✅ Within valid date range
- ✅ Has remaining usage limit
- ✅ Order meets minimum amount

---

## 🛠️ Admin Tasks (Coupon Management)

### Assign Coupon to User

**Request:** `POST /api/coupons/{id}/assign-user/{userId}`

```http
POST https://localhost:7286/api/coupons/1/assign-user/user-123
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "canUse": true
}
```

---

### Bulk Assign Coupon to Multiple Users

**Request:** `POST /api/coupons/{id}/assign-users`

```http
POST https://localhost:7286/api/coupons/1/assign-users
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
  "userIds": ["user-123", "user-456", "user-789"],
  "canUse": true
}
```

---

### Remove Coupon from User

**Request:** `DELETE /api/coupons/{id}/remove-user/{userId}`

```http
DELETE https://localhost:7286/api/coupons/1/remove-user/user-123
Authorization: Bearer {{authToken}}
```

---

## 📊 Webhook Verification

### Check Webhook Logs in Stripe CLI

When you run:
```bash
stripe listen --forward-to https://localhost:7286/api/webhooks/stripe
```

You'll see:
```
Ready! Your webhook signing secret is whsec_test_xxxx

2026-03-04 10:35:00 → payment_intent.succeeded [evt_test_xxxx]
2026-03-04 10:36:00 → charge.refunded [evt_test_xxxx]
```

---

## 🔍 Common Issues & Solutions

### Issue: "Order not found" error
**Solution:** Make sure you created order with authenticated user and copied correct `orderId`

### Issue: "You don't have access to this coupon"
**Solution:** Admin must assign coupon to user first via `/coupons/{id}/assign-user/{userId}`

### Issue: Webhook not firing
**Solution:** 
1. Ensure `stripe listen` is running in separate terminal
2. Check Stripe CLI is forwarding to correct URL: `https://localhost:7286/api/webhooks/stripe`
3. Verify webhook secret in `appsettings.json` matches CLI output

### Issue: "Payment intent not found"
**Solution:** Order and payment must be created with same authenticated user

---

## 📝 Request Collection Template

Save these requests in Postman for quick testing:

```
📁 Orders & Payments (Folder)
├── 🔐 Auth
│   ├── POST Login
│   └── GET Refresh Token
├── 📦 Orders
│   ├── POST Create Order
│   ├── GET Get Order By ID
│   ├── GET Get My Orders
│   └── POST Create Payment Intent
├── 💳 Coupons
│   ├── GET My Coupons
│   ├── GET Get Active Coupons
│   ├── POST Assign to User (Admin)
│   ├── POST Bulk Assign (Admin)
│   └── DELETE Remove from User (Admin)
└── 🔔 Webhooks
    └── POST Test Webhook (Manual trigger)
```

---

## ✅ Complete Testing Checklist

- [ ] User registered and authenticated
- [ ] Address added to profile
- [ ] Product available in catalog
- [ ] Order created with correct total
- [ ] Payment intent generated with `clientSecret`
- [ ] Payment confirmed in Stripe Dashboard
- [ ] Webhook received by application
- [ ] Order status changed to "Paid"
- [ ] Coupon applied correctly (discount calculated)
- [ ] User-specific coupon access working
- [ ] Failed payment handled correctly
- [ ] Refund processed successfully

---

## 🚀 Ready to Test!

1. ✅ Stripe CLI running: `stripe listen --forward-to https://localhost:7286/api/webhooks/stripe`
2. ✅ Application running: `dotnet run`
3. ✅ Postman environment configured
4. ✅ Start with Step 1️⃣ (Authentication)

**Questions?** Check `/STRIPE_SETUP_GUIDE.md` for detailed configuration.
