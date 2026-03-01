using ECommerce.DTOs.Products;

namespace ECommerce.DTOs.WishLists
{
    public class WishListItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
