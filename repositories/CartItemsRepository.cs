using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class CartItemsRepository : BaseRepository<CartItem>, ICartItemsRepository
    {
        public CartItemsRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<CartItem> Items, int TotalItems)> GetByCartIdAsync(int cartId, int page = 1, int pageSize = 10)
        {
            var query = _dbSet.Where(ci => ci.CartId == cartId)
                .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<CartItem?> GetByCartIdAndProductVariantIdAsync(int cartId, int productVariantId)
        {
            return await _dbSet
                .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductVariantId == productVariantId);
        }

        public override async Task<CartItem?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }
    }
}
