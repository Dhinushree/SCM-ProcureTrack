
using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class InventoryLedger
    {
        public int InventoryLedgerId { get; set; }

        public int ItemId { get; set; }

        public int? GoodsReceiptDetailId { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionType { get; set; } = "IN";

        public decimal Quantity { get; set; }

        public decimal BalanceAfter { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Remarks { get; set; }

        public Item? Item { get; set; }

        public GoodsReceiptDetail? GoodsReceiptDetail { get; set; }
    }
}
