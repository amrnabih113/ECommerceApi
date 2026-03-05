using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ProductVariantsRepository : BaseRepository<ProductVariant>, IProductVariantsRepository
    {
        public ProductVariantsRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<ProductVariant> Items, int TotalItems)> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet.Where(pv => pv.ProductId == productId);

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(pv => pv.Color)
                .ThenBy(pv => pv.Size)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<int> GetTotalStockByProductAsync(int productId)
        {
            return await _dbSet
                .Where(pv => pv.ProductId == productId)
                .SumAsync(pv => pv.StockQuantity);
        }

        public async Task<int> CountByProductAsync(int productId)
        {
            return await _dbSet
                .Where(pv => pv.ProductId == productId)
                .CountAsync();
        }
        public async Task<ProductVariant?> GetByIdWithLockAsync(int id)
        {
            return await _dbSet
                .FromSqlRaw("SELECT * FROM ProductVariants WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}", id)
                .Include(v => v.Product)
                .FirstOrDefaultAsync();
        }
    }
}