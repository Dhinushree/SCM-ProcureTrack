
using Xunit;
using DotNetAssignmentApp.Services;

namespace DotNetAssignmentApp.Tests
{
    public class PurchaseOrderCalculatorTests
    {
        private readonly PurchaseOrderCalculator _calculator
            = new PurchaseOrderCalculator();

        [Fact]
        public void CalculateSubtotal_ShouldReturnCorrectAmount()
        {
            var result = _calculator.CalculateSubtotal(100, 85);

            Assert.Equal(8500m, result);
        }

        [Fact]
        public void CalculateTax_ShouldReturnCorrectTax()
        {
            var result = _calculator.CalculateTax(8500, 18);

            Assert.Equal(1530m, result);
        }

        [Fact]
        public void CalculateGrandTotal_ShouldReturnSubtotalPlusTax()
        {
            var result = _calculator.CalculateGrandTotal(
                8500, 1530);

            Assert.Equal(10030m, result);
        }
    }
}
