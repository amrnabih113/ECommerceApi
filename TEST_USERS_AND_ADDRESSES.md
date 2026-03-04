# Test Users & Addresses - Seeded Data

All test users are automatically seeded to the database on first application startup.

## 👨‍💼 Admin User

| Field | Value |
|-------|-------|
| Email | `admin@ecommerce.com` |
| Password | `Admin@123` |
| Username | `admin` |
| Full Name | Admin User |
| Role | Admin |
| Phone | +1234567890 |

---

## 👥 Customer Users (4 Users)

### Customer 1 - John Doe

**Login Credentials:**

| Field | Value |
|-------|-------|
| Email | `customer@example.com` |
| Password | `Customer@123` |
| Username | `customer` |
| Full Name | John Doe |
| Role | User |
| Phone | +9876543210 |

**Addresses:** 3

| Address | City | State/Postal | Street |
|---------|------|-------------|--------|
| Home | New York | 10001 | 123 Main Street, Apt 4B |
| Work | Los Angeles | 90001 | 456 Oak Avenue, Suite 200 |
| Other | Chicago | 60601 | 789 Elm Street |

---

### Customer 2 - Jane Smith

**Login Credentials:**

| Field | Value |
|-------|-------|
| Email | `jane@example.com` |
| Password | `JaneSmith@123` |
| Username | `jane_smith` |
| Full Name | Jane Smith |
| Role | User |
| Phone | +1122334455 |

**Addresses:** 2

| Address | City | State/Postal | Street |
|---------|------|-------------|--------|
| Home | San Francisco | 94103 | 100 Market Street, Unit 5 |
| Office | Seattle | 98101 | 200 Pine Street |

---

### Customer 3 - Mike Johnson

**Login Credentials:**

| Field | Value |
|-------|-------|
| Email | `mike@example.com` |
| Password | `MikeJohnson@123` |
| Username | `mike_johnson` |
| Full Name | Mike Johnson |
| Role | User |
| Phone | +5544332211 |

**Addresses:** 3

| Address | City | State/Postal | Street |
|---------|------|-------------|--------|
| Home | Boston | 02210 | 300 Congress Street |
| Vacation | Miami | 33132 | 400 Biscayne Boulevard |
| Business | Denver | 80202 | 500 16th Street |

---

### Customer 4 - Sarah Williams

**Login Credentials:**

| Field | Value |
|-------|-------|
| Email | `sarah@example.com` |
| Password | `SarahWilliams@123` |
| Username | `sarah_williams` |
| Full Name | Sarah Williams |
| Role | User |
| Phone | +6677889900 |

**Addresses:** 2

| Address | City | State/Postal | Street |
|---------|------|-------------|--------|
| Home | Philadelphia | 19106 | 600 Market Street |
| Other | Houston | 77002 | 700 Louisiana Street |

---

## 🧪 Test with Postman

### Step 1: Login as Customer

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "customer@example.com",
  "password": "Customer@123"
}
```

**Response will include:**
- `token` (JWT token for authenticated requests)
- `refreshToken` (for refreshing expired tokens)
- `user` object with `id` (save this as {{userId}})

### Step 2: Get Your Addresses

```http
GET /api/addresses
Authorization: Bearer {{authToken}}
```

**Response:** List of 3 addresses for John Doe

### Step 3: Get Specific Address

```http
GET /api/addresses/1
Authorization: Bearer {{authToken}}
```

### Step 4: Create Order

```http
POST /api/orders
Authorization: Bearer {{authToken}}
Content-Type: application/json

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

---

## 🔄 Database Relationships

### User-Address (One-to-Many)
```
ApplicationUser (1) ──→ (Many) Address
- Each user has multiple addresses
- Each address belongs to one user
- Cascade delete: Deleting user deletes all their addresses
```

### User-Order (One-to-Many)
```
ApplicationUser (1) ──→ (Many) Order
- Each user has multiple orders
- Each order belongs to one user
- Cascade delete: Deleting user deletes all their orders
```

### User-Coupon (Many-to-Many)
```
ApplicationUser (Many) ──→ (via UserCoupon) ──→ (Many) Coupon
- Multiple users can have the same coupon
- Each user can have multiple coupons
- Tracks individual usage per user and globally
```

---

## ✅ Quick Testing Checklist

- [ ] Login as admin@ecommerce.com / Admin@123
- [ ] Login as customer@example.com / Customer@123
- [ ] Get addresses for customer (should return 3)
- [ ] Get specific address by ID
- [ ] Create address for customer
- [ ] Update address for customer
- [ ] Delete address for customer
- [ ] Create order with existing address
- [ ] Verify order belongs to user
- [ ] Test with other customer users

---

## 📝 Notes

- All passwords follow format: `Name@123` or custom (see above)
- All emails are confirmed (EmailConfirmed = true)
- All users have profile images from Unsplash
- Addresses use real US cities for better testing
- Database is recreated on each schema change (seeds run automatically)

---

**Last Updated:** March 4, 2026
