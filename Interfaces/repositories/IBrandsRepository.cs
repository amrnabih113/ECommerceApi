using ECommerce.Models;
using ECommerce.DTOs.Brands;

namespace ECommerce.Interfaces.Repositories
{
    public interface IBrandsRepository : IBaseRepository<Brand>
    {
        Task<(IEnumerable<Brand> Items, int TotalItems)> GetPagedAsync(BrandQueryDto query);
    }
}