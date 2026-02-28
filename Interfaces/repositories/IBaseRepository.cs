
using ECommerce.DTOs;

namespace ECommerce.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<(IEnumerable<T> Items, int TotalItems)> GetPagedAsync(int page = 1, int pageSize = 10);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}

