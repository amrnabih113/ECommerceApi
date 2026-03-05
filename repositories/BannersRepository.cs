using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class BannersRepository : BaseRepository<Banner>, IBannersRepository
    {
        public BannersRepository(AppDbContext context) : base(context) { }

        public override async Task<(IEnumerable<Banner> Items, int TotalItems)> GetPagedAsync(int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var query = _dbSet.OrderBy(b => b.DisplayOrder).ThenByDescending(b => b.CreatedAt).AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<IEnumerable<Banner>> GetActiveBannersOrderedAsync()
        {
            return await _dbSet
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ThenByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
