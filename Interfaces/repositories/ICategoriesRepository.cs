using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface ICategoriesRepository : IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> GetRootCategoriesAsync();

        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId);
    }
}