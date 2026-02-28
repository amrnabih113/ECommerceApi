

using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Products
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public string ImageUrl { get; set; } = default!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public int BrandId { get; set; }
    }
}