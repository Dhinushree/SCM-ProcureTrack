using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }

        [Required]
        [StringLength(30)]
        public string PONumber { get; set; } = string.Empty;

        public int VendorId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Today;

        public DateTime ExpectedDeliveryDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(30)]
        public string Status { get; set; } = "Draft";

        public decimal Subtotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrandTotal { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Vendor relationship
        public Vendor? Vendor { get; set; }

        // Purchase Order line items
        public ICollection<PurchaseOrderDetail> OrderDetails { get; set; }
            = new List<PurchaseOrderDetail>();
    }
}