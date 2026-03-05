using ECommerce.Models;
using ECommerce.DTOs.Products;

namespace ECommerce.Interfaces.Repositories
{
    public interface IProductsRepository : IBaseRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int TotalItems)> GetPagedAsync(ProductQueryDto query);

        Task<(IEnumerable<Product> Items, int TotalItems)> GetByCategoryIdAsync(int categoryId, ProductQueryDto query);

        Task<(IEnumerable<Product> Items, int TotalItems)> GetByBrandIdAsync(int brandId, ProductQueryDto query);

        Task<(IEnumerable<Product> Items, int TotalItems)> SearchAsync(string term, int page, int pageSize);
        Task<IEnumerable<string>> GetSearchRecommendationsAsync(string term, int size = 5);
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<int> ids);
        Task<(IEnumerable<Product> Items, int TotalItems)> GetSalesProductsAsync(ProductQueryDto query);
        Task<(IEnumerable<Product> Items, int TotalItems)> GetBestSalesProductsAsync(ProductQueryDto query);
        Task<Product?> GetByIdWithLockAsync(int id);
    }

}