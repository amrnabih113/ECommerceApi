
using ECommerce.Models;
namespace ECommerce.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByHashAsync(string hash);
        Task SaveAsync();
    }
}
