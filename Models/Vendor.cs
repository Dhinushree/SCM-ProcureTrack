using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class Vendor
    {
        public int VendorId { get; set; }

        [Required]
        [StringLength(20)]
        public string VendorCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? TaxGSTId { get; set; }

        [StringLength(50)]
        public string? PaymentTerms { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}