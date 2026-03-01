using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Products
{
    public class ProductVariantDto
    {
        public int Id { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Size { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Color { get; set; } = default!;

        public decimal? AdditionalPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; }
    }
}