using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ProductImagesRepository : BaseRepository<ProductImage>, IProductImagesRepository
    {
        public ProductImagesRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<ProductImage> Items, int TotalItems)> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet
                .Where(pi => pi.ProductId == productId)
                .OrderByDescending(pi => pi.IsMain)
                .ThenByDescending(pi => pi.CreatedAt);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<IEnumerable<ProductImage>> GetAllByProductIdAsync(int productId)
        {
            return await _dbSet
                .Where(pi => pi.ProductId == productId)
                .OrderByDescending(pi => pi.IsMain)
                .ThenByDescending(pi => pi.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductImage?> GetByProductIdAndImageIdAsync(int productId, int imageId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.Id == imageId);
        }
    }
}