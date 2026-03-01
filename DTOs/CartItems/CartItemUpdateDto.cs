using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.CartItems
{
    public class CartItemUpdateDto
    {
        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }
    }
}
