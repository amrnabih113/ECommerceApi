using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IReviewsRepository : IBaseRepository<Review>
    {
        Task<(IEnumerable<Review> Items, int TotalItems)> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10);
        Task<Review?> GetByProductIdAndUserIdAsync(int productId, string userId);
    }
}