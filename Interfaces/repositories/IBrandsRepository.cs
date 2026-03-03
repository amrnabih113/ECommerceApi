using ECommerce.Models;
using ECommerce.DTOs.Brands;

namespace ECommerce.Interfaces.Repositories
{
    public interface IBrandsRepository : IBaseRepository<Brand>
    {
        Task<(IEnumerable<Brand> Items, int TotalItems)> GetPagedAsync(BrandQueryDto query);
        Task<(IEnumerable<Brand> Items, int TotalItems)> SearchAsync(string term, int page, int pageSize);
        Task<IEnumerable<string>> GetSearchRecommendationsAsync(string term, int size = 5);
        Task<IEnumerable<Brand>> GetByIdsAsync(IEnumerable<int> ids);
    }
}