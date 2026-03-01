using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class CartsRepository : BaseRepository<Cart>, ICartsRepository
    {
        public CartsRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.AsNoTracking()
                .Include(c => c.Items)
                .ThenInclude(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
