
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DotNetAssignmentApp.Models;

namespace DotNetAssignmentApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<GoodsReceiptNote> GoodsReceiptNotes { get; set; }
        public DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; }
        public DbSet<InventoryLedger> InventoryLedgers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Item>()
                .Property(x => x.CurrentStock).HasPrecision(18, 3);

            modelBuilder.Entity<Item>()
                .Property(x => x.ReorderLevel).HasPrecision(18, 3);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(x => x.Subtotal).HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(x => x.TaxAmount).HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(x => x.GrandTotal).HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(x => x.OrderedQuantity).HasPrecision(18, 3);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(x => x.UnitPrice).HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(x => x.TaxPercentage).HasPrecision(5, 2);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(x => x.Amount).HasPrecision(18, 2);

            modelBuilder.Entity<GoodsReceiptDetail>()
                .Property(x => x.ReceivedQuantity).HasPrecision(18, 3);

            modelBuilder.Entity<GoodsReceiptDetail>()
                .Property(x => x.AcceptedQuantity).HasPrecision(18, 3);

            modelBuilder.Entity<GoodsReceiptDetail>()
                .Property(x => x.RejectedQuantity).HasPrecision(18, 3);

            modelBuilder.Entity<InventoryLedger>()
                .Property(x => x.Quantity).HasPrecision(18, 3);

            modelBuilder.Entity<InventoryLedger>()
                .Property(x => x.BalanceAfter).HasPrecision(18, 3);

            // Prevent SQL Server multiple cascade paths
            modelBuilder.Entity<GoodsReceiptDetail>()
                .HasOne(x => x.GoodsReceiptNote)
                .WithMany(x => x.ReceiptDetails)
                .HasForeignKey(x => x.GoodsReceiptNoteId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<GoodsReceiptDetail>()
                .HasOne(x => x.PurchaseOrderDetail)
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderDetailId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
