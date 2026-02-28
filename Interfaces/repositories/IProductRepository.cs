using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IProductsRepository : IBaseRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int TotalItems)> GetByCategoryIdAsync(int categoryId, int page = 1, int pageSize = 10);

        Task<(IEnumerable<Product> Items, int TotalItems)> GetByBrandIdAsync(int brandId, int page = 1, int pageSize = 10);

        //  Task<IEnumerable<Product>> SearchAsync(string query);
    }

}