using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class GoodsReceiptNote
    {
        public int GoodsReceiptNoteId { get; set; }

        [Required]
        [StringLength(30)]
        public string GRNNumber { get; set; } = string.Empty;

        public int PurchaseOrderId { get; set; }

        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Draft";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationship
        public PurchaseOrder? PurchaseOrder { get; set; }

        public ICollection<GoodsReceiptDetail> ReceiptDetails { get; set; }
            = new List<GoodsReceiptDetail>();
    }
}