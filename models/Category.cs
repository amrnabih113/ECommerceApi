
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ECommerce.Models
{

    public class Category : Entity
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [AllowNull]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Url]
        [Required]
        public required string ImageUrl { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(ParentCategoryId))]
        public int ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();


    }
}