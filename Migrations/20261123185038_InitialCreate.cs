using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetAssignmentApp.Migrations;

/// <inheritdoc />
public partial class _20261123185038_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Items",
            columns: table => new
            {
                ItemId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ItemCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                ItemName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CurrentStock = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                ReorderLevel = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Items", x => x.ItemId);
            });

        migrationBuilder.CreateTable(
            name: "Vendors",
            columns: table => new
            {
                VendorId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                VendorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TaxGSTId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                PaymentTerms = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vendors", x => x.VendorId);
            });

        migrationBuilder.CreateTable(
            name: "PurchaseOrders",
            columns: table => new
            {
                PurchaseOrderId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PONumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                VendorId = table.Column<int>(type: "int", nullable: false),
                OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                GrandTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PurchaseOrders", x => x.PurchaseOrderId);
                table.ForeignKey(
                    name: "FK_PurchaseOrders_Vendors_VendorId",
                    column: x => x.VendorId,
                    principalTable: "Vendors",
                    principalColumn: "VendorId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GoodsReceiptNotes",
            columns: table => new
            {
                GoodsReceiptNoteId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GRNNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                ReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsReceiptNotes", x => x.GoodsReceiptNoteId);
                table.ForeignKey(
                    name: "FK_GoodsReceiptNotes_PurchaseOrders_PurchaseOrderId",
                    column: x => x.PurchaseOrderId,
                    principalTable: "PurchaseOrders",
                    principalColumn: "PurchaseOrderId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PurchaseOrderDetails",
            columns: table => new
            {
                PurchaseOrderDetailId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                ItemId = table.Column<int>(type: "int", nullable: false),
                OrderedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TaxPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PurchaseOrderDetails", x => x.PurchaseOrderDetailId);
                table.ForeignKey(
                    name: "FK_PurchaseOrderDetails_Items_ItemId",
                    column: x => x.ItemId,
                    principalTable: "Items",
                    principalColumn: "ItemId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderId",
                    column: x => x.PurchaseOrderId,
                    principalTable: "PurchaseOrders",
                    principalColumn: "PurchaseOrderId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GoodsReceiptDetails",
            columns: table => new
            {
                GoodsReceiptDetailId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GoodsReceiptNoteId = table.Column<int>(type: "int", nullable: false),
                PurchaseOrderDetailId = table.Column<int>(type: "int", nullable: false),
                ReceivedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                AcceptedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                RejectedQuantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                InspectionRemarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoodsReceiptDetails", x => x.GoodsReceiptDetailId);
                table.ForeignKey(
                    name: "FK_GoodsReceiptDetails_GoodsReceiptNotes_GoodsReceiptNoteId",
                    column: x => x.GoodsReceiptNoteId,
                    principalTable: "GoodsReceiptNotes",
                    principalColumn: "GoodsReceiptNoteId");
                table.ForeignKey(
                    name: "FK_GoodsReceiptDetails_PurchaseOrderDetails_PurchaseOrderDetailId",
                    column: x => x.PurchaseOrderDetailId,
                    principalTable: "PurchaseOrderDetails",
                    principalColumn: "PurchaseOrderDetailId");
            });

        migrationBuilder.CreateTable(
            name: "InventoryLedgers",
            columns: table => new
            {
                InventoryLedgerId = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ItemId = table.Column<int>(type: "int", nullable: false),
                GoodsReceiptDetailId = table.Column<int>(type: "int", nullable: true),
                TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                BalanceAfter = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryLedgers", x => x.InventoryLedgerId);
                table.ForeignKey(
                    name: "FK_InventoryLedgers_GoodsReceiptDetails_GoodsReceiptDetailId",
                    column: x => x.GoodsReceiptDetailId,
                    principalTable: "GoodsReceiptDetails",
                    principalColumn: "GoodsReceiptDetailId");
                table.ForeignKey(
                    name: "FK_InventoryLedgers_Items_ItemId",
                    column: x => x.ItemId,
                    principalTable: "Items",
                    principalColumn: "ItemId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceiptDetails_GoodsReceiptNoteId",
            table: "GoodsReceiptDetails",
            column: "GoodsReceiptNoteId");

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceiptDetails_PurchaseOrderDetailId",
            table: "GoodsReceiptDetails",
            column: "PurchaseOrderDetailId");

        migrationBuilder.CreateIndex(
            name: "IX_GoodsReceiptNotes_PurchaseOrderId",
            table: "GoodsReceiptNotes",
            column: "PurchaseOrderId");

        migrationBuilder.CreateIndex(
            name: "IX_InventoryLedgers_GoodsReceiptDetailId",
            table: "InventoryLedgers",
            column: "GoodsReceiptDetailId");

        migrationBuilder.CreateIndex(
            name: "IX_InventoryLedgers_ItemId",
            table: "InventoryLedgers",
            column: "ItemId");

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrderDetails_ItemId",
            table: "PurchaseOrderDetails",
            column: "ItemId");

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrderDetails_PurchaseOrderId",
            table: "PurchaseOrderDetails",
            column: "PurchaseOrderId");

        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrders_VendorId",
            table: "PurchaseOrders",
            column: "VendorId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InventoryLedgers");

        migrationBuilder.DropTable(
            name: "GoodsReceiptDetails");

        migrationBuilder.DropTable(
            name: "GoodsReceiptNotes");

        migrationBuilder.DropTable(
            name: "PurchaseOrderDetails");

        migrationBuilder.DropTable(
            name: "Items");

        migrationBuilder.DropTable(
            name: "PurchaseOrders");

        migrationBuilder.DropTable(
            name: "Vendors");
    }
}
