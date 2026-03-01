using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Products
{
    public class ProductVariantUpdateDto
    {
        [Url(ErrorMessage = "Invalid URL format.")]
        public string? ImageUrl { get; set; }

        [MaxLength(100, ErrorMessage = "Size cannot exceed 100 characters.")]
        public string? Size { get; set; }

        [MaxLength(100, ErrorMessage = "Color cannot exceed 100 characters.")]
        public string? Color { get; set; }

        [Range(0.01, 9999999, ErrorMessage = "Additional price must be greater than 0.")]
        public decimal? AdditionalPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int? StockQuantity { get; set; }
    }
}
