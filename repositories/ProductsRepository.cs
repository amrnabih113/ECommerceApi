using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ProductsRepository : BaseRepository<Product>, IProductsRepository
    {
        public ProductsRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<Product> Items, int TotalItems)> GetByBrandIdAsync(int brandId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet.Where(p => p.BrandId == brandId);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }



        public async Task<(IEnumerable<Product> Items, int TotalItems)> GetByCategoryIdAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet.Where(p => p.CategoryId == categoryId);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }
    }
}