
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
        [ForeignKey(nameof(ProductVariant))]
        public int ProductVariantId { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;

        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; set; } = null!;
    }
}