
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Product : Entity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 9999999)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        public bool HasDiscount { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAfterDiscount { get; set; }

        [NotMapped]
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(CategoryId))]
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [ForeignKey(nameof(BrandId))]
        [Required]
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}