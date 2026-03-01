using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IWishListRepository : IBaseRepository<WishList>
    {
        Task<WishList?> GetByUserAndProductAsync(string userId, int productId);
        Task<(IEnumerable<WishList> Items, int TotalItems)> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10);
        Task<bool> ExistsAsync(string userId, int productId);
    }
}
