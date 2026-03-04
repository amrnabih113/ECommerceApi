using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IAddressesRepository : IBaseRepository<Address>
    {
        Task<List<Address>> GetByUserIdAsync(string userId);
        Task<Address?> GetByIdAndUserIdAsync(int id, string userId);
    }
}
