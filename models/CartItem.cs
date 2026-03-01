
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class CartItem : Entity
    {

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [ForeignKey(nameof(ProductVariant))]
        public int? ProductVariantId { get; set; }

        public ProductVariant? ProductVariant { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; set; } = null!;

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}