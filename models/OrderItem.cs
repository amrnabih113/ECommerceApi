
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class OrderItem : Entity
    {
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}