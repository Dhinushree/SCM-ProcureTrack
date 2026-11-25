using Xunit;
using DotNetAssignmentApp.Models;

namespace DotNetAssignmentApp.Tests
{
    public class VendorTests
    {
        [Fact]
        public void Vendor_ShouldHaveCorrectCompanyName()
        {
            // Arrange
            var vendor = new Vendor
            {
                VendorCode = "VEN001",
                CompanyName = "ABC Industrial Supplies Pvt Ltd"
            };

            // Assert
            Assert.Equal(
                "ABC Industrial Supplies Pvt Ltd",
                vendor.CompanyName);
        }

        [Fact]
        public void Vendor_ShouldBeActiveByDefault()
        {
            // Arrange
            var vendor = new Vendor();

            // Assert
            Assert.True(vendor.IsActive);
        }

        [Fact]
        public void Vendor_ShouldStoreCorrectVendorCode()
        {
            // Arrange
            var vendor = new Vendor
            {
                VendorCode = "VEN002"
            };

            // Assert
            Assert.Equal("VEN002", vendor.VendorCode);
        }
    }
}