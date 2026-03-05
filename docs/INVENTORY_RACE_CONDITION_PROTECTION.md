# Inventory Race Condition Protection

## Overview
This implementation prevents race conditions when multiple users simultaneously attempt to purchase the same product, especially the last item in stock.

## The Problem
**Without protection:**
```
Time    User A                          User B
----    ------                          ------
T1      Read stock: 1 item             
T2                                      Read stock: 1 item
T3      Check OK, reduce stock to 0    
T4                                      Check OK, reduce stock to -1 ❌
T5      Order created                   Order created
Result: BOTH orders succeed, stock = -1 (CRITICAL BUG)
```

## The Solution
**With row-level locking:**
```
Time    User A                          User B
----    ------                          ------
T1      Lock row, read stock: 1        
T2                                      Wait for lock... ⏳
T3      Check OK, reduce stock to 0    
T4      Commit & release lock          
T5                                      Lock acquired, read stock: 0
T6                                      Check FAILS ✅
Result: User A succeeds, User B gets "out of stock" error
```

---

## Implementation Details

### 1. Row-Level Locking

#### IProductVariantsRepository.cs
```csharp
/// <summary>
/// Gets a product variant with row-level locking to prevent race conditions.
/// Must be called within a transaction.
/// </summary>
Task<ProductVariant?> GetByIdWithLockAsync(int id);
```

#### ProductVariantsRepository.cs
```csharp
public async Task<ProductVariant?> GetByIdWithLockAsync(int id)
{
    // UPDLOCK: Prevents other transactions from acquiring update or exclusive locks
    // ROWLOCK: Locks only this specific row, not the entire table
    return await _dbSet
        .FromSqlRaw("SELECT * FROM ProductVariants WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id)
        .Include(v => v.Product)
        .FirstOrDefaultAsync();
}
```

**SQL Server Lock Hints:**
- **UPDLOCK**: Prevents other transactions from acquiring update or exclusive locks (read is OK)
- **ROWLOCK**: Locks only the specific row, not the entire table (better concurrency)

### 2. Products Without Variants

Similar implementation for products that don't have variants:

#### IProductRepository.cs
```csharp
Task<Product?> GetByIdWithLockAsync(int id);
```

#### ProductsRepository.cs
```csharp
public async Task<Product?> GetByIdWithLockAsync(int id)
{
    return await _dbSet
        .FromSqlRaw("SELECT * FROM Products WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id)
        .FirstOrDefaultAsync();
}
```

### 3. OrdersService Stock Reduction

The critical path in `CreateOrderAsync`:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    foreach (var item in dto.OrderItems)
    {
        if (item.ProductVariantId.HasValue)
        {
            // ✅ RACE CONDITION PROTECTION: Lock the row
            var variant = await _productVariantsRepository.GetByIdWithLockAsync(
                item.ProductVariantId.Value
            );

            // ✅ ATOMIC CHECK: Verify stock within the locked transaction
            if (variant.StockQuantity < item.Quantity)
            {
                await transaction.RollbackAsync();
                return ApiResponse<OrderDto>.Error(
                    $"Insufficient stock. Available: {variant.StockQuantity}"
                );
            }

            // ✅ PREVENT NEGATIVE STOCK: Reduce only after validation
            variant.StockQuantity -= item.Quantity;
        }
        else
        {
            // Same logic for products without variants
            var product = await _productsRepository.GetByIdWithLockAsync(item.ProductId);
            
            if (product.StockQuantity < item.Quantity)
            {
                await transaction.RollbackAsync();
                return ApiResponse<OrderDto>.Error("Insufficient stock");
            }
            
            product.StockQuantity -= item.Quantity;
        }
    }
    
    // Create order and commit
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---

## Database-Level Protection

### Check Constraints
Additional safety net at the database level:

```sql
-- Prevent negative stock in ProductVariants
ALTER TABLE ProductVariants
ADD CONSTRAINT CK_ProductVariants_StockQuantity_NonNegative
CHECK (StockQuantity >= 0);

-- Prevent negative stock in Products
ALTER TABLE Products
ADD CONSTRAINT CK_Products_StockQuantity_NonNegative
CHECK (StockQuantity >= 0);

-- Ensure positive quantities in orders
ALTER TABLE OrderItems
ADD CONSTRAINT CK_OrderItems_Quantity_Positive
CHECK (Quantity > 0);
```

To apply these constraints:
```bash
sqlcmd -S localhost -d ECommerceDB -i Migrations/AddStockCheckConstraints.sql
```

---

## Testing Race Conditions

### Load Test Scenario
```bash
# Simulate 100 concurrent users buying the last item
ab -n 100 -c 100 -p order.json -T application/json \
   https://localhost:7286/api/orders
```

### Expected Results
- ✅ **1 order succeeds** (stock reduced from 1 to 0)
- ✅ **99 orders fail** with "Insufficient stock" error
- ✅ **Stock never goes negative** (stays at 0)

### Without Protection (Before)
- ❌ Multiple orders succeed
- ❌ Stock becomes negative (-50, -100, etc.)
- ❌ Overselling occurs

---

## Performance Considerations

### Lock Duration
Row locks are held only for:
1. Reading stock quantity
2. Validating stock
3. Reducing stock
4. Committing transaction

**Typical lock hold time: < 50ms**

### Concurrency Impact
- ✅ Different products: No blocking (different rows)
- ✅ Different variants: No blocking (different rows)
- ⚠️ Same product/variant: Sequential processing (by design)

### Optimization Tips
```csharp
// ✅ GOOD: Lock acquired late, held briefly
var cart = await GetCart();
var coupon = await ValidateCoupon();
// Lock acquired here ⬇️
var variant = await GetByIdWithLockAsync(id);
variant.StockQuantity -= qty;
await SaveChangesAsync();
// Lock released here ⬆️

// ❌ BAD: Lock held for entire order process
var variant = await GetByIdWithLockAsync(id); // Lock acquired
var cart = await GetCart(); // Still holding lock
var coupon = await ValidateCoupon(); // Still holding lock
variant.StockQuantity -= qty;
await SaveChangesAsync(); // Lock finally released
```

---

## Security Benefits

### 1. Prevents Overselling
- Cannot sell more than available stock
- Critical for physical product businesses

### 2. Data Integrity
- Stock count always accurate
- No negative inventory

### 3. Financial Protection
- Prevents fulfillment of orders that can't be completed
- Avoids refund/compensation costs

### 4. User Trust
- Customers never buy "out of stock" items
- Professional error handling

---

## Best Practices

### ✅ DO
- Always use transactions for stock reduction
- Lock rows before reading stock
- Validate stock within the locked transaction
- Rollback on any validation failure
- Use specific error messages
- Log race condition events

### ❌ DON'T
- Read stock outside transaction
- Check stock without locking
- Reduce stock before validation
- Use table-level locks (kills concurrency)
- Allow negative stock
- Ignore lock timeout errors

---

## Monitoring & Alerts

### Key Metrics
```csharp
// Log when stock prevents order
_logger.LogWarning(
    "Order blocked: Insufficient stock for variant {VariantId}. " +
    "Available: {Available}, Requested: {Requested}",
    variantId, available, requested
);

// Log potential high-contention scenarios
_logger.LogInformation(
    "High contention detected for product {ProductId}. " +
    "Lock wait time: {WaitTimeMs}ms",
    productId, waitTime
);
```

### Dashboard Queries
```sql
-- Products with frequent stock-outs
SELECT p.Id, p.Name, COUNT(*) as StockOutCount
FROM Products p
JOIN AuditLog a ON a.ProductId = p.Id
WHERE a.Message LIKE '%Insufficient stock%'
GROUP BY p.Id, p.Name
ORDER BY StockOutCount DESC;

-- High-contention products (frequent simultaneous access)
SELECT ProductId, COUNT(*) as ConcurrentAttempts, AVG(LockWaitTime) as AvgWaitMs
FROM OrderAttemptLog
WHERE CreatedAt > DATEADD(hour, -1, GETUTCDATE())
GROUP BY ProductId
HAVING COUNT(*) > 10
ORDER BY ConcurrentAttempts DESC;
```

---

## Summary

| Protection Layer | Mechanism | Purpose |
|-----------------|-----------|---------|
| **Application** | Row-level locking (UPDLOCK) | Prevent race conditions |
| **Application** | Transaction validation | Atomic stock checks |
| **Application** | Explicit stock checks | Business logic validation |
| **Database** | Check constraints | Prevent negative stock |
| **Database** | Transaction isolation | ACID guarantees |

**Result:** Rock-solid inventory management even under high concurrent load! 🔒

---

## Migration Checklist

- [x] Add `GetByIdWithLockAsync` to `IProductVariantsRepository`
- [x] Implement row-level locking in `ProductVariantsRepository`
- [x] Add `GetByIdWithLockAsync` to `IProductsRepository`
- [x] Implement row-level locking in `ProductsRepository`
- [x] Inject repositories into `OrdersService`
- [x] Update `CreateOrderAsync` to use locked methods
- [x] Add atomic stock validation
- [x] Add explicit rollback on stock failures
- [x] Create SQL check constraints script
- [ ] Apply SQL check constraints to database
- [ ] Load test with concurrent users
- [ ] Monitor for lock contention
- [ ] Update API documentation
