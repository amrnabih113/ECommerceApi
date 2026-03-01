

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
        public bool HasVariants { get; set; } = false;
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; } = 0;
        [Required]
        public int CategoryId { get; set; }

        public int BrandId { get; set; }
    }
}