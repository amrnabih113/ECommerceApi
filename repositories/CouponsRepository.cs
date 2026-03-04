using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class CouponsRepository : BaseRepository<Coupon>, ICouponsRepository
    {
        public CouponsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Code.ToLower() == code.ToLower());

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<(IEnumerable<Coupon> Items, int TotalItems)> GetActiveCouponsAsync(int page, int pageSize)
        {
            var query = _dbSet
                .Where(c => c.IsActive && c.ValidFrom <= DateTime.UtcNow && c.ValidUntil >= DateTime.UtcNow)
                .OrderBy(c => c.ValidUntil);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task IncrementUsedCountAsync(int couponId)
        {
            var coupon = await _dbSet.FindAsync(couponId);
            if (coupon != null)
            {
                coupon.UsedCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}
