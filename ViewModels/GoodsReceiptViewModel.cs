
using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.ViewModels
{
    public class GoodsReceiptViewModel
    {
        [Required]
        public int PurchaseOrderId { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        [StringLength(500)]
        public string? Remarks { get; set; }

        public List<GoodsReceiptItemViewModel> Items { get; set; }
            = new List<GoodsReceiptItemViewModel>();
    }

    public class GoodsReceiptItemViewModel
    {
        public int PurchaseOrderDetailId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public decimal OrderedQuantity { get; set; }

        public decimal PreviouslyAcceptedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ReceivedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AcceptedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RejectedQuantity { get; set; }

        [StringLength(500)]
        public string? InspectionRemarks { get; set; }
    }
}
