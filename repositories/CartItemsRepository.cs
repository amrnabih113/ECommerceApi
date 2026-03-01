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
                .Include(ci => ci.Product)
                .Include(ci => ci.ProductVariant);

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
                .Include(ci => ci.Product)
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductVariantId == productVariantId);
        }

        public async Task<CartItem?> GetByCartIdAndProductIdAsync(int cartId, int productId)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId && ci.ProductVariantId == null);
        }

        public override async Task<CartItem?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<int> CountByProductVariantAsync(int productVariantId)
        {
            return await _dbSet.CountAsync(ci => ci.ProductVariantId == productVariantId);
        }
    }
}
