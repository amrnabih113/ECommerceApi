using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class CategoriesRepository
        : BaseRepository<Category>, ICategoriesRepository
    {
        public CategoriesRepository(AppDbContext context)
            : base(context)
        {
        }

        public override async Task<(IEnumerable<Category> Items, int TotalItems)> GetPagedAsync(
               int page,
               int pageSize
           )
        {
            var skip = (page - 1) * pageSize;
            var items = await _dbSet
                .Where(c => c.IsActive)
                .Include(c => c.Products)
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var totalItems = await _dbSet.CountAsync(c => c.IsActive);

            return (items, totalItems);
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == null && c.IsActive).Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentId)
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == parentId && c.IsActive)
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}