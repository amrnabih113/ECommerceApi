-- Add check constraints to prevent negative stock quantities
-- This is a database-level safety net in addition to application-level validation

-- Add check constraint for ProductVariants.StockQuantity
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ProductVariants_StockQuantity_NonNegative')
BEGIN
    ALTER TABLE ProductVariants
    ADD CONSTRAINT CK_ProductVariants_StockQuantity_NonNegative
    CHECK (StockQuantity >= 0);
END
GO

-- Add check constraint for Products.StockQuantity
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Products_StockQuantity_NonNegative')
BEGIN
    ALTER TABLE Products
    ADD CONSTRAINT CK_Products_StockQuantity_NonNegative
    CHECK (StockQuantity >= 0);
END
GO

-- Add check constraint for CartItems.Quantity
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_CartItems_Quantity_Positive')
BEGIN
    ALTER TABLE CartItems
    ADD CONSTRAINT CK_CartItems_Quantity_Positive
    CHECK (Quantity > 0);
END
GO

-- Add check constraint for OrderItems.Quantity
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_OrderItems_Quantity_Positive')
BEGIN
    ALTER TABLE OrderItems
    ADD CONSTRAINT CK_OrderItems_Quantity_Positive
    CHECK (Quantity > 0);
END
GO

PRINT 'Stock quantity check constraints added successfully!';
