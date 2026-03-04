using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface ICouponsRepository : IBaseRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<(IEnumerable<Coupon> Items, int TotalItems)> GetActiveCouponsAsync(int page, int pageSize);
        Task IncrementUsedCountAsync(int couponId);
    }
}
