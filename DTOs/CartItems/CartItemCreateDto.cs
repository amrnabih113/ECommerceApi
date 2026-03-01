using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.CartItems
{
    public class CartItemCreateDto
    {
        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        public int ProductVariantId { get; set; }
    }
}
