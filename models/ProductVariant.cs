
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class ProductVariant : Entity
    {
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        [Required]
        [Url]
        public string ImageUrl { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Size { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Color { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AdditionalPrice { get; set; }

        [Required]
        public int StockQuantity { get; set; }
    }
}