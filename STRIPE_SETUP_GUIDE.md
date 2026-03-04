# Stripe Payment Integration Setup Guide

## Overview
This e-commerce application uses Stripe for payment processing with the following features:
- **Payment Intent Flow**: Create → Confirm → Webhook
- **Payment Methods**: Card, Bank Transfer, Wallet
- **Refund Support**: Full and partial refunds
- **Webhook Events**: Real-time payment status updates

---

## Step 1: Create a Stripe Account

1. Visit [https://stripe.com](https://stripe.com)
2. Click **Sign up** and create your account
3. Complete the account verification process
4. Once logged in, you'll be in **Test Mode** by default

---

## Step 2: Get Your API Keys

1. Go to **Dashboard** → **Developers** → **API keys**
2. You'll see two types of keys:
   - **Publishable key** (starts with `pk_test_`): Used in frontend/client
   - **Secret key** (starts with `sk_test_`): Used in backend (keep private!)

3. Copy both keys and add them to your `appsettings.json`:

```json
"Stripe": {
  "PublishableKey": "pk_test_YOUR_ACTUAL_KEY_HERE",
  "SecretKey": "sk_test_YOUR_ACTUAL_KEY_HERE",
  "WebhookSecret": "whsec_YOUR_WEBHOOK_SECRET_HERE"
}
```

⚠️ **Security Note**: Never commit `appsettings.json` with real keys to version control!

---

## Step 3: Install Stripe .NET SDK

Run the following command in your project directory:

```bash
dotnet add package Stripe.net
```

✅ This package is already configured in the `StripeService.cs` file.

---

## Step 4: Configure Webhook Endpoint

Webhooks allow Stripe to notify your application about payment events in real-time.

### 4.1: Setup Local Testing with Stripe CLI

For local development, use the Stripe CLI to forward webhooks to localhost:

1. **Download Stripe CLI**:
   - Visit [https://stripe.com/docs/stripe-cli](https://stripe.com/docs/stripe-cli)
   - Download and install for Windows

2. **Login to Stripe CLI**:
   ```bash
   stripe login
   ```

3. **Forward webhooks to your local server** (in a NEW PowerShell terminal):
   ```bash
   stripe listen --forward-to https://localhost:7286/api/webhooks/stripe
   ```
   
   You'll see output like:
   ```
   > Ready! Your webhook signing secret is whsec_test_secret_here
   ```
   
4. **Copy the webhook signing secret** (starts with `whsec_`)

5. **Add to your LOCAL `appsettings.json`**:
   ```json
   "Stripe": {
     "PublishableKey": "pk_test_YOUR_TEST_KEY",
     "SecretKey": "sk_test_YOUR_TEST_KEY",
     "WebhookSecret": "whsec_test_secret_here"
   }
   ```

6. **Keep the stripe listen terminal running** while you test

### 4.1.1: Testing Payment Flow Locally

✅ **Your application is now set up for local webhook testing**

1. **Start your app**:
   ```bash
   dotnet run
   ```

2. **Create an order**:
   ```bash
   POST /api/orders
   {
     "addressId": 1,
     "orderItems": [
       {
         "productId": 1,
         "quantity": 2
       }
     ]
   }
   ```
   Response: `{ "data": { "id": 123, "status": "Pending" } }`

3. **Create payment intent**:
   ```bash
   POST /api/orders/123/payment
   ```
   Response:
   ```json
   {
     "success": true,
     "data": {
       "clientSecret": "pi_test_secret_here",
       "paymentIntentId": "pi_test_id"
     }
   }
   ```

4. **Simulate payment in Stripe Dashboard**:
   - Go to [https://dashboard.stripe.com/test/payments](https://dashboard.stripe.com/test/payments)
   - Find the payment intent (shows as "Processing")
   - Click on it
   - Click "Confirm Payment"
   - **Webhook automatically fires** → Order status changes to "Paid"

5. **Verify in your app**:
   ```bash
   GET /api/orders/123
   ```
   Status should now be: `"Paid"` ✅

**What happens behind the scenes**:
```
Your App              Stripe CLI            Stripe Servers        Database
   |                    |                        |                    |
   |-- POST /orders ----|                        |                    |
   |                    |                        |                    |
   |-- POST /payment----|                        |                    |
   |                    |                        |                    |
   |                    |<-- You confirm in dashboard              |
   |                    |                        |                    |
   |                    |<-- payment_intent.succeeded               |
   |                    |                        |                    |
   |<-- Forward webhook-|                        |                    |
   |                    |                        |                    |
   |-- Update order status ---------------------->|
   |                    |                        |
   ✅ Order marked as "Paid"
```


### 4.2: Setup Production Webhooks

For production deployment only (not for local development):

⚠️ **Important**: Stripe Dashboard cannot access `localhost` or private IPs. You MUST have a publicly accessible domain.

1. Go to **Dashboard** → **Developers** → **Webhooks**
2. Click **Add endpoint**
3. Enter your webhook URL: `https://yourdomain.com/api/webhooks/stripe`
   - Replace `yourdomain.com` with your actual domain
   - Must be HTTPS and publicly accessible
4. Select the following events to listen to:
   - `payment_intent.succeeded`
   - `payment_intent.payment_failed`
   - `charge.refunded`

5. Click **Add endpoint**
6. Copy the **Signing secret** (starts with `whsec_`)
7. Add it to your production `appsettings.json`

**For Local Development**: Do NOT add webhook in Stripe Dashboard. Instead, use Stripe CLI (Section 4.1) which can forward events to localhost.

---

## Step 5: Database Migration

Create and apply the migration for Payment, Order, and Coupon tables:

```bash
dotnet ef migrations add AddPaymentSystemTables
dotnet ef database update
```

This will create:
- **Payments** table with Stripe integration fields
- Update **Orders** table with payment relationship
- Update **Coupons** table with percentage/fixed discount support

---

## Step 6: Test the Integration

### 6.1: Test Card Numbers (Test Mode)

Use these test card numbers in test mode:

| Card Number         | Description          | Expected Result |
|---------------------|----------------------|-----------------|
| 4242 4242 4242 4242 | Visa                 | Success         |
| 4000 0000 0000 0002 | Visa                 | Declined        |
| 4000 0025 0000 3155 | Visa (requires 3DS)  | Success with 3DS|

- **Expiry**: Any future date (e.g., 12/25)
- **CVC**: Any 3 digits (e.g., 123)
- **ZIP**: Any 5 digits (e.g., 12345)

### 6.2: Test Payment Flow

1. **Create an order** via POST `/api/orders`
2. **Create a payment intent** via POST `/api/orders/{orderId}/payment`
3. **Get the ClientSecret** from the response
4. Use the ClientSecret in your frontend to complete the payment with Stripe.js
5. **Webhook will automatically update** order and payment status

---

## Step 7: Frontend Integration (Stripe.js)

Add Stripe.js to your frontend application:

```html
<script src="https://js.stripe.com/v3/"></script>
```

Example payment confirmation:

```javascript
const stripe = Stripe('pk_test_YOUR_PUBLISHABLE_KEY');

// After getting clientSecret from backend
const { error } = await stripe.confirmCardPayment(clientSecret, {
  payment_method: {
    card: cardElement,
    billing_details: {
      name: 'Customer Name',
    },
  },
});

if (error) {
  console.error(error.message);
} else {
  // Payment succeeded - webhook will update order status
  console.log('Payment successful!');
}
```

---

## API Endpoints

### Orders
- `GET /api/orders` - Get all orders (Admin only)
- `GET /api/orders/my-orders` - Get current user's orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create a new order
- `PATCH /api/orders/{id}/status` - Update order status (Admin)
- `POST /api/orders/{id}/cancel` - Cancel an order
- `POST /api/orders/{id}/payment` - Create payment intent for order

### Coupons
- `GET /api/coupons` - Get all coupons (Admin only)
- `GET /api/coupons/active` - Get active coupons
- `GET /api/coupons/{id}` - Get coupon by ID
- `GET /api/coupons/code/{code}` - Get coupon by code
- `POST /api/coupons/validate` - Validate a coupon code
- `POST /api/coupons` - Create coupon (Admin)
- `PUT /api/coupons/{id}` - Update coupon (Admin)
- `DELETE /api/coupons/{id}` - Delete coupon (Admin)

### Webhooks
- `POST /api/webhooks/stripe` - Stripe webhook handler

---

## Payment Flow Diagram

```
1. User creates order → Order Status: Pending
2. System creates PaymentIntent → Payment Status: Pending
3. Frontend confirms payment with Stripe
4. Stripe processes payment
5. Webhook receives payment_intent.succeeded → Payment Status: Succeeded, Order Status: Paid
6. Admin ships order → Order Status: Shipped
7. Admin marks delivered → Order Status: Delivered
```

---

## Order Status Flow

- **Pending** → Order created, awaiting payment
- **Paid** → Payment confirmed
- **Shipped** → Order shipped to customer
- **Delivered** → Order delivered
- **Cancelled** → Order cancelled (refund initiated if paid)

---

## Coupon System

### Discount Types:
1. **Percentage** (0-100%): e.g., 20% off
2. **Fixed Amount**: e.g., $10 off

### Validation Rules:
- Active status
- Valid date range (ValidFrom - ValidUntil)
- Usage limit (UsageLimit - UsedCount)
- Minimum order amount
- Maximum discount cap (for percentage coupons)

---

## Security Best Practices

1. **Never expose Secret Key** in client-side code
2. **Always verify webhook signatures** (already implemented in `WebhooksController`)
3. **Use HTTPS** in production
4. **Store sensitive data** in environment variables or Azure Key Vault
5. **Enable Stripe Radar** for fraud detection in production
6. **Implement idempotency keys** for payment retries (already handled by Stripe)

---

## Troubleshooting

### Webhook not receiving events locally
- Ensure `stripe listen` is running
- Check firewall settings
- Verify URL: `https://localhost:7286/api/webhooks/stripe`

### Payment fails with "Invalid API Key"
- Check `Stripe:SecretKey` in appsettings.json
- Ensure you're using test keys in development

### Webhook signature verification fails
- Verify `Stripe:WebhookSecret` matches the one from Stripe CLI or Dashboard
- Check that webhook endpoint is using raw request body

---

## Production Checklist

Before going live:
- [ ] Switch to **live mode** API keys (start with `pk_live_` and `sk_live_`)
- [ ] Configure production webhook endpoint
- [ ] Enable HTTPS on your domain
- [ ] Test with real payment methods
- [ ] Enable Stripe Radar for fraud prevention
- [ ] Setup email notifications for failed payments
- [ ] Configure refund policies
- [ ] Test webhook delivery and retries

---

## Support

- **Stripe Documentation**: [https://stripe.com/docs](https://stripe.com/docs)
- **Stripe Support**: [https://support.stripe.com](https://support.stripe.com)
- **Test Cards**: [https://stripe.com/docs/testing](https://stripe.com/docs/testing)

---

## Summary

Your payment system is now ready! The implementation includes:
✅ Complete order management
✅ Flexible coupon system (percentage/fixed discounts)
✅ Stripe PaymentIntent integration
✅ Webhook handling for real-time updates
✅ Refund support (full/partial)
✅ Stock management during order creation
✅ Transaction-based operations with rollback

**Next steps**: Update appsettings.json with your Stripe keys, run migrations, and test!
