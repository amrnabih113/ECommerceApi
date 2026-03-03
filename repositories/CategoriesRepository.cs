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

        public async Task<(IEnumerable<Category> Items, int TotalItems)> SearchAsync(string term, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var normalizedTerm = term.Trim();

            try
            {
                var fullTextQuery = _dbSet
                    .Include(c => c.Products)
                    .Where(c =>
                        EF.Functions.FreeText(c.Name, normalizedTerm) ||
                        (c.Description != null && EF.Functions.FreeText(c.Description, normalizedTerm)))
                    .OrderBy(c => c.Name)
                    .ThenBy(c => c.Id)
                    .AsNoTracking();

                var totalItems = await fullTextQuery.CountAsync();
                var items = await fullTextQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalItems);
            }
            catch
            {
                var fallbackTerm = normalizedTerm.ToLower();
                var fallbackQuery = _dbSet
                    .Include(c => c.Products)
                    .Where(c =>
                        c.Name.ToLower().Contains(fallbackTerm) ||
                        (c.Description != null && c.Description.ToLower().Contains(fallbackTerm)))
                    .OrderBy(c => c.Name)
                    .ThenBy(c => c.Id)
                    .AsNoTracking();

                var totalItems = await fallbackQuery.CountAsync();
                var items = await fallbackQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (items, totalItems);
            }
        }

        public async Task<IEnumerable<string>> GetSearchRecommendationsAsync(string term, int size = 5)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Enumerable.Empty<string>();

            var normalizedTerm = term.Trim().ToLower();

            return await _dbSet
                .Where(c => c.Name.ToLower().StartsWith(normalizedTerm))
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .Distinct()
                .Take(size)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idSet = ids.Distinct().ToList();
            if (!idSet.Any()) return Enumerable.Empty<Category>();

            return await _dbSet
                .Include(c => c.Products)
                .Where(c => idSet.Contains(c.Id))
                .AsNoTracking()
                .ToListAsync();
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