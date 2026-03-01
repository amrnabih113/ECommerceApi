using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface ICartsRepository : IBaseRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(string userId);
    }
}

