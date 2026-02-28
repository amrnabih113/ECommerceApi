using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Categories
{
    public class CategoryUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}