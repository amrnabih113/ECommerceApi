using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface ICartItemsRepository : IBaseRepository<CartItem>
    {
        Task<(IEnumerable<CartItem> Items, int TotalItems)> GetByCartIdAsync(int cartId, int page = 1, int pageSize = 10);
        Task<CartItem?> GetByCartIdAndProductVariantIdAsync(int cartId, int productVariantId);
    }
}
