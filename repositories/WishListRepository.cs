using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class WishListRepository : BaseRepository<WishList>, IWishListRepository
    {
        public WishListRepository(AppDbContext context) : base(context) { }

        public async Task<WishList?> GetByUserAndProductAsync(string userId, int productId)
        {
            return await _dbSet
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<(IEnumerable<WishList> Items, int TotalItems)> GetByUserIdAsync(string userId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ThenInclude(p => p!.Images)
                .Include(w => w.Product)
                .ThenInclude(p => p!.Category)
                .Include(w => w.Product)
                .ThenInclude(p => p!.Brand)
                .OrderByDescending(w => w.CreatedAt);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<bool> ExistsAsync(string userId, int productId)
        {
            return await _dbSet
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}
