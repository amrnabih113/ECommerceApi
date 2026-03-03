using ECommerce.Data;
using ECommerce.DTOs.Categories;
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
            var query = new CategoryQueryDto
            {
                Page = page,
                PageSize = pageSize
            };

            return await GetPagedAsync(query);
        }

        public async Task<(IEnumerable<Category> Items, int TotalItems)> GetPagedAsync(CategoryQueryDto query)
        {
            query.Page = query.Page <= 0 ? 1 : query.Page;
            query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var categoriesQuery = _dbSet
                .Include(c => c.Products)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                categoriesQuery = categoriesQuery.Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    (c.Description != null && c.Description.ToLower().Contains(search)));
            }

            if (query.IsActive.HasValue)
                categoriesQuery = categoriesQuery.Where(c => c.IsActive == query.IsActive.Value);
            else
                categoriesQuery = categoriesQuery.Where(c => c.IsActive);

            if (query.ParentCategoryId.HasValue)
                categoriesQuery = categoriesQuery.Where(c => c.ParentCategoryId == query.ParentCategoryId.Value);

            var sortBy = query.SortBy?.Trim().ToLowerInvariant();
            var sortDesc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            categoriesQuery = sortBy switch
            {
                "name" => sortDesc ? categoriesQuery.OrderByDescending(c => c.Name) : categoriesQuery.OrderBy(c => c.Name),
                "createdat" => sortDesc ? categoriesQuery.OrderByDescending(c => c.CreatedAt) : categoriesQuery.OrderBy(c => c.CreatedAt),
                "productscount" => sortDesc
                    ? categoriesQuery.OrderByDescending(c => c.Products.Count(p => p.IsActive))
                    : categoriesQuery.OrderBy(c => c.Products.Count(p => p.IsActive)),
                _ => sortDesc ? categoriesQuery.OrderByDescending(c => c.Id) : categoriesQuery.OrderBy(c => c.Id)
            };

            var totalItems = await categoriesQuery.CountAsync();

            var items = await categoriesQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .AsNoTracking()
                .ToListAsync();


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