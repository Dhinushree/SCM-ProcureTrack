
USE SCMProcureTrackDB;
GO

/* =====================================================
   1. GET OPEN PURCHASE ORDERS BY VENDOR
===================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_GetOpenPurchaseOrdersByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PO.PurchaseOrderId,
        PO.PONumber,
        V.CompanyName,
        PO.OrderDate,
        PO.ExpectedDeliveryDate,
        PO.Status,
        PO.GrandTotal
    FROM dbo.PurchaseOrders PO
    INNER JOIN dbo.Vendors V
        ON PO.VendorId = V.VendorId
    WHERE PO.VendorId = @VendorId
      AND PO.Status IN ('Approved', 'Partially Received')
    ORDER BY PO.OrderDate DESC;
END;
GO


/* =====================================================
   2. CALCULATE PURCHASE ORDER TOTALS
===================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_CalculatePurchaseOrderTotals
    @PurchaseOrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PurchaseOrders
        WHERE PurchaseOrderId = @PurchaseOrderId
    )
    BEGIN
        RAISERROR('Purchase Order not found.', 16, 1);
        RETURN;
    END;

    DECLARE @Subtotal DECIMAL(18,2);
    DECLARE @TaxAmount DECIMAL(18,2);
    DECLARE @GrandTotal DECIMAL(18,2);

    SELECT
        @Subtotal = ISNULL(
            SUM(OrderedQuantity * UnitPrice), 0
        ),
        @TaxAmount = ISNULL(
            SUM(
                OrderedQuantity * UnitPrice
                * TaxPercentage / 100
            ), 0
        )
    FROM dbo.PurchaseOrderDetails
    WHERE PurchaseOrderId = @PurchaseOrderId;

    SET @GrandTotal = @Subtotal + @TaxAmount;

    UPDATE dbo.PurchaseOrders
    SET
        Subtotal = @Subtotal,
        TaxAmount = @TaxAmount,
        GrandTotal = @GrandTotal
    WHERE PurchaseOrderId = @PurchaseOrderId;

    SELECT
        @Subtotal AS Subtotal,
        @TaxAmount AS TaxAmount,
        @GrandTotal AS GrandTotal;
END;
GO


/* =====================================================
   3. VALIDATE GRN QUANTITIES
===================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_ValidateGRNQuantities
    @PurchaseOrderDetailId INT,
    @NewAcceptedQuantity DECIMAL(18,3)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OrderedQuantity DECIMAL(18,3);
    DECLARE @PreviouslyAccepted DECIMAL(18,3);

    SELECT
        @OrderedQuantity = OrderedQuantity
    FROM dbo.PurchaseOrderDetails
    WHERE PurchaseOrderDetailId = @PurchaseOrderDetailId;

    IF @OrderedQuantity IS NULL
    BEGIN
        RAISERROR('Invalid Purchase Order detail.', 16, 1);
        RETURN;
    END;

    IF @NewAcceptedQuantity IS NULL
       OR @NewAcceptedQuantity < 0
    BEGIN
        RAISERROR(
            'Accepted quantity cannot be NULL or negative.',
            16,
            1
        );
        RETURN;
    END;

    SELECT
        @PreviouslyAccepted =
            ISNULL(SUM(GRD.AcceptedQuantity), 0)
    FROM dbo.GoodsReceiptDetails GRD
    INNER JOIN dbo.GoodsReceiptNotes GRN
        ON GRD.GoodsReceiptNoteId =
           GRN.GoodsReceiptNoteId
    WHERE GRD.PurchaseOrderDetailId =
          @PurchaseOrderDetailId
      AND GRN.Status = 'Posted';

    IF @PreviouslyAccepted + @NewAcceptedQuantity
       > @OrderedQuantity
    BEGIN
        RAISERROR(
            'Accepted quantity exceeds ordered quantity.',
            16,
            1
        );
        RETURN;
    END;

    SELECT
        @OrderedQuantity AS OrderedQuantity,
        @PreviouslyAccepted AS PreviouslyAcceptedQuantity,
        @NewAcceptedQuantity AS NewAcceptedQuantity,
        'Valid' AS ValidationStatus;
END;
GO


/* =====================================================
   4. UPDATE ITEM STOCK AND INVENTORY LEDGER
===================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_UpdateItemStock
    @ItemId INT,
    @AcceptedQuantity DECIMAL(18,3)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Items
        WHERE ItemId = @ItemId
    )
    BEGIN
        RAISERROR('Item not found.', 16, 1);
        RETURN;
    END;

    IF @AcceptedQuantity IS NULL
       OR @AcceptedQuantity <= 0
    BEGIN
        RAISERROR(
            'Accepted quantity must be greater than zero.',
            16,
            1
        );
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Items
        SET CurrentStock = CurrentStock + @AcceptedQuantity
        WHERE ItemId = @ItemId;

        INSERT INTO dbo.InventoryLedgers
        (
            ItemId,
            TransactionType,
            Quantity,
            BalanceAfter,
            ReferenceNumber,
            TransactionDate,
            Remarks
        )
        SELECT
            ItemId,
            'IN',
            @AcceptedQuantity,
            CurrentStock,
            'MANUAL-STOCK-UPDATE',
            GETUTCDATE(),
            'Stock updated through stored procedure'
        FROM dbo.Items
        WHERE ItemId = @ItemId;

        COMMIT TRANSACTION;

        SELECT
            ItemId,
            CurrentStock
        FROM dbo.Items
        WHERE ItemId = @ItemId;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        DECLARE @ErrorMessage NVARCHAR(4000);

        SET @ErrorMessage = ERROR_MESSAGE();

        RAISERROR(@ErrorMessage, 16, 1);
        RETURN;

    END CATCH;
END;
GO


/* =====================================================
   5. UPDATE PURCHASE ORDER STATUS
===================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_UpdatePurchaseOrderStatus
    @PurchaseOrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PurchaseOrders
        WHERE PurchaseOrderId = @PurchaseOrderId
    )
    BEGIN
        RAISERROR('Purchase Order not found.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.PurchaseOrders
        WHERE PurchaseOrderId = @PurchaseOrderId
          AND Status IN ('Approved', 'Partially Received')
    )
    BEGIN
        RAISERROR(
            'Only Approved or Partially Received orders can be updated.',
            16,
            1
        );
        RETURN;
    END;

    DECLARE @AllCompleted BIT = 1;
    DECLARE @AnyReceived BIT = 0;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.PurchaseOrderDetails POD
        WHERE POD.PurchaseOrderId = @PurchaseOrderId
          AND
          (
              SELECT ISNULL(
                  SUM(GRD.AcceptedQuantity), 0
              )
              FROM dbo.GoodsReceiptDetails GRD
              INNER JOIN dbo.GoodsReceiptNotes GRN
                  ON GRD.GoodsReceiptNoteId =
                     GRN.GoodsReceiptNoteId
              WHERE GRD.PurchaseOrderDetailId =
                    POD.PurchaseOrderDetailId
                AND GRN.Status = 'Posted'
          ) < POD.OrderedQuantity
    )
    BEGIN
        SET @AllCompleted = 0;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.GoodsReceiptDetails GRD
        INNER JOIN dbo.GoodsReceiptNotes GRN
            ON GRD.GoodsReceiptNoteId =
               GRN.GoodsReceiptNoteId
        INNER JOIN dbo.PurchaseOrderDetails POD
            ON GRD.PurchaseOrderDetailId =
               POD.PurchaseOrderDetailId
        WHERE POD.PurchaseOrderId = @PurchaseOrderId
          AND GRN.Status = 'Posted'
          AND GRD.AcceptedQuantity > 0
    )
    BEGIN
        SET @AnyReceived = 1;
    END;

    IF @AllCompleted = 1 AND @AnyReceived = 1
    BEGIN
        UPDATE dbo.PurchaseOrders
        SET Status = 'Completed'
        WHERE PurchaseOrderId = @PurchaseOrderId;
    END
    ELSE IF @AnyReceived = 1
    BEGIN
        UPDATE dbo.PurchaseOrders
        SET Status = 'Partially Received'
        WHERE PurchaseOrderId = @PurchaseOrderId;
    END;

    SELECT
        PurchaseOrderId,
        PONumber,
        Status
    FROM dbo.PurchaseOrders
    WHERE PurchaseOrderId = @PurchaseOrderId;
END;
GO


/* =====================================================
   6. GET DASHBOARD SUMMARY
===================================================== */

CREATE OR ALTER PROCEDURE dbo.usp_GetDashboardSummary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (
            SELECT COUNT(*)
            FROM dbo.Vendors
            WHERE IsActive = 1
        ) AS TotalVendors,

        (
            SELECT COUNT(*)
            FROM dbo.PurchaseOrders
        ) AS TotalPurchaseOrders,

        (
            SELECT COUNT(*)
            FROM dbo.PurchaseOrders
            WHERE Status IN
            (
                'Approved',
                'Partially Received'
            )
        ) AS OpenPurchaseOrders,

        (
            SELECT ISNULL(
                SUM(
                    (POD.OrderedQuantity -
                     ISNULL(Received.AcceptedQuantity, 0))
                    * POD.UnitPrice
                    * (1 + POD.TaxPercentage / 100)
                ), 0
            )
            FROM dbo.PurchaseOrderDetails POD
            INNER JOIN dbo.PurchaseOrders PO
                ON POD.PurchaseOrderId =
                   PO.PurchaseOrderId
            OUTER APPLY
            (
                SELECT
                    SUM(GRD.AcceptedQuantity)
                        AS AcceptedQuantity
                FROM dbo.GoodsReceiptDetails GRD
                INNER JOIN dbo.GoodsReceiptNotes GRN
                    ON GRD.GoodsReceiptNoteId =
                       GRN.GoodsReceiptNoteId
                WHERE GRD.PurchaseOrderDetailId =
                      POD.PurchaseOrderDetailId
                  AND GRN.Status = 'Posted'
            ) Received
            WHERE PO.Status IN
            (
                'Approved',
                'Partially Received'
            )
        ) AS PendingDeliveryValue,

        (
            SELECT COUNT(*)
            FROM dbo.Items
            WHERE CurrentStock <= ReorderLevel
              AND IsActive = 1
        ) AS LowStockItems;
END;
GO


PRINT 'All six stored procedures created successfully.';
GO
