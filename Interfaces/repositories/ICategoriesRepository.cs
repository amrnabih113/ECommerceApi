using ECommerce.Models;
using ECommerce.DTOs.Categories;

namespace ECommerce.Interfaces.Repositories
{
    public interface ICategoriesRepository : IBaseRepository<Category>
    {
        Task<(IEnumerable<Category> Items, int TotalItems)> GetPagedAsync(CategoryQueryDto query);
        Task<(IEnumerable<Category> Items, int TotalItems)> SearchAsync(string term, int page, int pageSize);
        Task<IEnumerable<string>> GetSearchRecommendationsAsync(string term, int size = 5);
        Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<int> ids);

        Task<IEnumerable<Category>> GetRootCategoriesAsync();

        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
    }
}