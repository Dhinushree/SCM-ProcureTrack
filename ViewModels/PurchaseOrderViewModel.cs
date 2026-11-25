
using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.ViewModels
{
    public class PurchaseOrderViewModel
    {
        [Required]
        public int VendorId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime ExpectedDeliveryDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        public List<PurchaseOrderItemViewModel> Items { get; set; }
            = new List<PurchaseOrderItemViewModel>();
    }

    public class PurchaseOrderItemViewModel
    {
        [Required]
        public int ItemId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal OrderedQuantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal TaxPercentage { get; set; }
    }
}
