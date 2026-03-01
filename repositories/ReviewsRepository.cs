using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ReviewsRepository : BaseRepository<Review>, IReviewsRepository
    {
        public ReviewsRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<Review> Items, int TotalItems)> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<Review?> GetByProductIdAndUserIdAsync(int productId, string userId)
        {
            return await _dbSet
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);
        }

        public override async Task<Review?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}