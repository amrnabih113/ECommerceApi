using ECommerce.Models;
using ECommerce.DTOs.Products;

namespace ECommerce.Interfaces.Repositories
{
    public interface IProductsRepository : IBaseRepository<Product>
    {
        Task<(IEnumerable<Product> Items, int TotalItems)> GetPagedAsync(ProductQueryDto query);

        Task<(IEnumerable<Product> Items, int TotalItems)> GetByCategoryIdAsync(int categoryId, ProductQueryDto query);

        Task<(IEnumerable<Product> Items, int TotalItems)> GetByBrandIdAsync(int brandId, ProductQueryDto query);

        //  Task<IEnumerable<Product>> SearchAsync(string query);
    }

}