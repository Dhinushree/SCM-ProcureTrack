using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class PurchaseOrderDetail
    {
        public int PurchaseOrderDetailId { get; set; }

        public int PurchaseOrderId { get; set; }

        public int ItemId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal OrderedQuantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal TaxPercentage { get; set; }

        public decimal Amount { get; set; }

        // Relationships
        public PurchaseOrder? PurchaseOrder { get; set; }

        public Item? Item { get; set; }
    }
}