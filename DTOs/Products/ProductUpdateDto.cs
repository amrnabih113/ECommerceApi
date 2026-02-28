using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Products
{
    public class ProductUpdateDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = default!;
        [Required]
        [Url]
        public string ImageUrl { get; set; } = default!;
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}