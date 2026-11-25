
namespace DotNetAssignmentApp.Services
{
    public class PurchaseOrderCalculator
    {
        public decimal CalculateSubtotal(
            decimal quantity,
            decimal unitPrice)
        {
            return Math.Round(
                quantity * unitPrice, 2);
        }

        public decimal CalculateTax(
            decimal subtotal,
            decimal taxPercentage)
        {
            return Math.Round(
                subtotal * taxPercentage / 100, 2);
        }

        public decimal CalculateGrandTotal(
            decimal subtotal,
            decimal taxAmount)
        {
            return subtotal + taxAmount;
        }
    }
}
