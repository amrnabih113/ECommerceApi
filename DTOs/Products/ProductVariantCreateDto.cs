using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Products
{
    public class ProductVariantCreateDto
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string ImageUrl { get; set; } = default!;

        [Required(ErrorMessage = "Size is required.")]
        [MaxLength(100, ErrorMessage = "Size cannot exceed 100 characters.")]
        public string Size { get; set; } = default!;

        [Required(ErrorMessage = "Color is required.")]
        [MaxLength(100, ErrorMessage = "Color cannot exceed 100 characters.")]
        public string Color { get; set; } = default!;

        [Range(0.01, 9999999, ErrorMessage = "Additional price must be greater than 0.")]
        public decimal? AdditionalPrice { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }
    }
}
