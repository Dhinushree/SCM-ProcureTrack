
/*
    Project  : SCM ProcureTrack
    File     : SeedData.sql
    Database : SCMProcureTrackDB

    Purpose:
    Insert sample Vendors and Items.
    Existing records will not be overwritten.
*/

USE SCMProcureTrackDB;
GO

/* =========================================
   1. INSERT SAMPLE VENDORS
========================================= */

IF NOT EXISTS (
    SELECT 1 FROM dbo.Vendors
    WHERE VendorCode = 'VEN001'
)
BEGIN
    INSERT INTO dbo.Vendors
    (
        VendorCode, CompanyName, ContactPerson,
        Email, Phone, TaxGSTId, PaymentTerms,
        IsActive, CreatedAt
    )
    VALUES
    (
        'VEN001', 'ABC Industrial Supplies Pvt Ltd',
        'Arun Kumar', 'arun@abc.com', '9876543210',
        '33ABCDE1234F1Z5', 'Net 30', 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Vendors
    WHERE VendorCode = 'VEN002'
)
BEGIN
    INSERT INTO dbo.Vendors
    (
        VendorCode, CompanyName, ContactPerson,
        Email, Phone, TaxGSTId, PaymentTerms,
        IsActive, CreatedAt
    )
    VALUES
    (
        'VEN002', 'Global Packaging Solutions',
        'Priya', 'priya@globalpack.com', '9876543211',
        '33ABCDE2234F1Z5', 'Net 15', 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Vendors
    WHERE VendorCode = 'VEN003'
)
BEGIN
    INSERT INTO dbo.Vendors
    (
        VendorCode, CompanyName, ContactPerson,
        Email, Phone, TaxGSTId, PaymentTerms,
        IsActive, CreatedAt
    )
    VALUES
    (
        'VEN003', 'Prime Electrical Components',
        'Karthik', 'karthik@primeelec.com', '9876543212',
        '33ABCDE3234F1Z5', 'Net 45', 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Vendors
    WHERE VendorCode = 'VEN004'
)
BEGIN
    INSERT INTO dbo.Vendors
    (
        VendorCode, CompanyName, ContactPerson,
        Email, Phone, TaxGSTId, PaymentTerms,
        IsActive, CreatedAt
    )
    VALUES
    (
        'VEN004', 'Sunrise Raw Materials',
        'Meena', 'meena@sunriserm.com', '9876543213',
        '33ABCDE4234F1Z5', 'Net 60', 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Vendors
    WHERE VendorCode = 'VEN005'
)
BEGIN
    INSERT INTO dbo.Vendors
    (
        VendorCode, CompanyName, ContactPerson,
        Email, Phone, TaxGSTId, PaymentTerms,
        IsActive, CreatedAt
    )
    VALUES
    (
        'VEN005', 'Smart Office Supplies',
        'Suresh', 'suresh@smartoffice.com', '9876543214',
        '33ABCDE5234F1Z5', 'Immediate', 1, GETUTCDATE()
    );
END;
GO


/* =========================================
   2. INSERT SAMPLE ITEMS
========================================= */

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM001'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM001', 'Industrial Steel Sheets',
        'Raw Materials', 'Kg', 250, 100, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM002'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM002', 'Plastic Packaging Bags',
        'Packaging', 'Nos', 1200, 500, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM003'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM003', 'Electrical Cables',
        'Electrical', 'Meter', 450, 200, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM004'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM004', 'Copper Wire Rolls',
        'Raw Materials', 'Roll', 45, 20, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM005'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM005', 'Corrugated Carton Boxes',
        'Packaging', 'Nos', 700, 300, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM006'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM006', 'Industrial Bolts & Nuts',
        'Hardware', 'Nos', 600, 250, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM007'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM007', 'Safety Gloves',
        'Safety Equipment', 'Pair', 180, 100, 1, GETUTCDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM dbo.Items
    WHERE ItemCode = 'ITM008'
)
BEGIN
    INSERT INTO dbo.Items
    (
        ItemCode, ItemName, Category,
        UnitOfMeasure, CurrentStock,
        ReorderLevel, IsActive, CreatedAt
    )
    VALUES
    (
        'ITM008', 'Lubricating Oil',
        'Maintenance', 'Litre', 90, 50, 1, GETUTCDATE()
    );
END;
GO


/* =========================================
   3. DISPLAY VENDORS
========================================= */

SELECT
    VendorCode,
    CompanyName,
    ContactPerson,
    PaymentTerms,
    IsActive
FROM dbo.Vendors
ORDER BY VendorCode;
GO


/* =========================================
   4. DISPLAY ITEMS
========================================= */

SELECT
    ItemCode,
    ItemName,
    Category,
    UnitOfMeasure,
    CurrentStock,
    ReorderLevel
FROM dbo.Items
ORDER BY ItemCode;
GO


PRINT 'SCM ProcureTrack seed data script completed successfully.';
GO
