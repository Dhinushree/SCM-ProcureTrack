
using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class GoodsReceiptDetail
    {
        public int GoodsReceiptDetailId { get; set; }

        public int GoodsReceiptNoteId { get; set; }

        public int PurchaseOrderDetailId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal ReceivedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AcceptedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RejectedQuantity { get; set; }

        [StringLength(500)]
        public string? InspectionRemarks { get; set; }

        public GoodsReceiptNote? GoodsReceiptNote { get; set; }

        public PurchaseOrderDetail? PurchaseOrderDetail { get; set; }
    }
}
