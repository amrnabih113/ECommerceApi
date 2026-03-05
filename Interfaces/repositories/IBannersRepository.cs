using ECommerce.Models;
using ECommerce.DTOs;

namespace ECommerce.Interfaces.Repositories
{
    public interface IBannersRepository : IBaseRepository<Banner>
    {
        new Task<(IEnumerable<Banner> Items, int TotalItems)> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<Banner>> GetActiveBannersOrderedAsync();
    }
}
