
using Xunit;
using DotNetAssignmentApp.Services;

namespace DotNetAssignmentApp.Tests
{
    public class GoodsReceiptValidatorTests
    {
        private readonly GoodsReceiptValidator _validator =
            new GoodsReceiptValidator();

        [Fact]
        public void IsReceiptQuantityValid_ShouldReturnTrue_WhenQuantitiesMatch()
        {
            var result = _validator.IsReceiptQuantityValid(
                received: 60,
                accepted: 55,
                rejected: 5);

            Assert.True(result);
        }

        [Fact]
        public void IsAcceptedQuantityWithinOrder_ShouldReturnFalse_WhenAcceptedExceedsOrdered()
        {
            var result = _validator.IsAcceptedQuantityWithinOrder(
                orderedQuantity: 100,
                previouslyAccepted: 55,
                newlyAccepted: 50);

            Assert.False(result);
        }
    }
}
