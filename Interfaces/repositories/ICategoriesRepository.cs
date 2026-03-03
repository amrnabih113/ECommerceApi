using ECommerce.Models;
using ECommerce.DTOs.Categories;

namespace ECommerce.Interfaces.Repositories
{
    public interface ICategoriesRepository : IBaseRepository<Category>
    {
        Task<(IEnumerable<Category> Items, int TotalItems)> GetPagedAsync(CategoryQueryDto query);

        Task<IEnumerable<Category>> GetRootCategoriesAsync();

        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
    }
}