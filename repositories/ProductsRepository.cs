using ECommerce.Data;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ProductsRepository : BaseRepository<Product>, IProductsRepository
    {
        public ProductsRepository(AppDbContext context) : base(context) { }

        public async Task<(IEnumerable<Product> Items, int TotalItems)> GetPagedAsync(ProductQueryDto query)
        {
            query.Page = query.Page <= 0 ? 1 : query.Page;
            query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var productsQuery = BuildQuery(query);

            var totalItems = await productsQuery.CountAsync();

            var items = await productsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<(IEnumerable<Product> Items, int TotalItems)> GetByBrandIdAsync(int brandId, ProductQueryDto query)
        {
            query.Page = query.Page <= 0 ? 1 : query.Page;
            query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            query.BrandId = brandId;
            var productsQuery = BuildQuery(query);

            var totalItems = await productsQuery.CountAsync();

            var items = await productsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<(IEnumerable<Product> Items, int TotalItems)> GetByCategoryIdAsync(int categoryId, ProductQueryDto query)
        {
            query.Page = query.Page <= 0 ? 1 : query.Page;
            query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            query.CategoryId = categoryId;
            var productsQuery = BuildQuery(query);

            var totalItems = await productsQuery.CountAsync();

            var items = await productsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        private IQueryable<Product> BuildQuery(ProductQueryDto query)
        {
            IQueryable<Product> productsQuery = _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Variants);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Description.ToLower().Contains(search));
            }

            if (query.BrandId.HasValue)
                productsQuery = productsQuery.Where(p => p.BrandId == query.BrandId.Value);

            if (query.CategoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.CategoryId == query.CategoryId.Value);

            if (query.IsActive.HasValue)
                productsQuery = productsQuery.Where(p => p.IsActive == query.IsActive.Value);

            if (query.HasVariants.HasValue)
                productsQuery = productsQuery.Where(p => p.HasVariants == query.HasVariants.Value);

            if (query.MinPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice.Value);

            var sortBy = query.SortBy?.Trim().ToLowerInvariant();
            var sortDesc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            productsQuery = sortBy switch
            {
                "name" => sortDesc ? productsQuery.OrderByDescending(p => p.Name) : productsQuery.OrderBy(p => p.Name),
                "price" => sortDesc ? productsQuery.OrderByDescending(p => p.Price) : productsQuery.OrderBy(p => p.Price),
                "createdat" => sortDesc ? productsQuery.OrderByDescending(p => p.CreatedAt) : productsQuery.OrderBy(p => p.CreatedAt),
                _ => sortDesc ? productsQuery.OrderByDescending(p => p.Id) : productsQuery.OrderBy(p => p.Id)
            };

            return productsQuery.AsNoTracking();
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}