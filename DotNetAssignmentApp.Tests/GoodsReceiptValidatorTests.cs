
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
            var result = _validator.IsReceiptQuantityValid(60, 55, 5);
            Assert.True(result);
        }

        [Fact]
        public void IsReceiptQuantityValid_ShouldReturnFalse_WhenReceivedIsNegative()
        {
            var result = _validator.IsReceiptQuantityValid(-10, 5, 5);
            Assert.False(result);
        }

        [Fact]
        public void IsReceiptQuantityValid_ShouldReturnFalse_WhenAcceptedIsNegative()
        {
            var result = _validator.IsReceiptQuantityValid(10, -5, 15);
            Assert.False(result);
        }

        [Fact]
        public void IsReceiptQuantityValid_ShouldReturnFalse_WhenRejectedIsNegative()
        {
            var result = _validator.IsReceiptQuantityValid(10, 15, -5);
            Assert.False(result);
        }

        [Fact]
        public void IsReceiptQuantityValid_ShouldReturnFalse_WhenQuantitiesDoNotMatch()
        {
            var result = _validator.IsReceiptQuantityValid(60, 50, 5);
            Assert.False(result);
        }

        [Fact]
        public void IsAcceptedQuantityWithinOrder_ShouldReturnFalse_WhenAcceptedExceedsOrdered()
        {
            var result = _validator.IsAcceptedQuantityWithinOrder(100, 55, 50);
            Assert.False(result);
        }

        [Fact]
        public void IsAcceptedQuantityWithinOrder_ShouldReturnTrue_WhenAcceptedEqualsOrdered()
        {
            var result = _validator.IsAcceptedQuantityWithinOrder(100, 70, 30);
            Assert.True(result);
        }

        [Fact]
        public void IsAcceptedQuantityWithinOrder_ShouldReturnTrue_WhenAcceptedIsBelowOrdered()
        {
            var result = _validator.IsAcceptedQuantityWithinOrder(100, 50, 30);
            Assert.True(result);
        }

        [Fact]
        public void IsAcceptedQuantityWithinOrder_ShouldReturnFalse_WhenOrderedIsNegative()
        {
            var result = _validator.IsAcceptedQuantityWithinOrder(-100, 0, 10);
            Assert.False(result);
        }
    }
}
