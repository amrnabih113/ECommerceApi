using ECommerce.DTOs.CartItems;

namespace ECommerce.DTOs.Carts
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = default!;
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
