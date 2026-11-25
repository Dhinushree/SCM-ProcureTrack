using System.ComponentModel.DataAnnotations;

namespace DotNetAssignmentApp.Models
{
    public class Item
    {
        public int ItemId { get; set; }

        [Required]
        [StringLength(30)]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }

        [Required]
        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = string.Empty;

        public decimal CurrentStock { get; set; } = 0;

        public decimal ReorderLevel { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}