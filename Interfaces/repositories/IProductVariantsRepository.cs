using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IProductVariantsRepository : IBaseRepository<ProductVariant>
    {
        Task<(IEnumerable<ProductVariant> Items, int TotalItems)> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10);
        Task<int> GetTotalStockByProductAsync(int productId);
        Task<int> CountByProductAsync(int productId);
    }
}
