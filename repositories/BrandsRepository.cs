using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Data;
using ECommerce.DTOs.Brands;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class BrandsRepository : BaseRepository<Brand>, IBrandsRepository
    {
        public BrandsRepository(AppDbContext context) : base(context)
        {

        }

        public override async Task<(IEnumerable<Brand> Items, int TotalItems)> GetPagedAsync(
              int page,
              int pageSize
          )
        {
            var query = new BrandQueryDto
            {
                Page = page,
                PageSize = pageSize
            };

            return await GetPagedAsync(query);
        }

        public async Task<(IEnumerable<Brand> Items, int TotalItems)> GetPagedAsync(BrandQueryDto query)
        {
            query.Page = query.Page <= 0 ? 1 : query.Page;
            query.PageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var brandsQuery = _dbSet
                .Include(c => c.Products)
                .AsQueryable();

            if (query.MinProductsCount.HasValue)
                brandsQuery = brandsQuery.Where(b => b.Products.Count(p => p.IsActive) >= query.MinProductsCount.Value);

            var sortBy = query.SortBy?.Trim().ToLowerInvariant();
            var sortDesc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            brandsQuery = sortBy switch
            {
                "name" => sortDesc ? brandsQuery.OrderByDescending(b => b.Name) : brandsQuery.OrderBy(b => b.Name),
                "createdat" => sortDesc ? brandsQuery.OrderByDescending(b => b.CreatedAt) : brandsQuery.OrderBy(b => b.CreatedAt),
                "productscount" => sortDesc
                    ? brandsQuery.OrderByDescending(b => b.Products.Count(p => p.IsActive))
                    : brandsQuery.OrderBy(b => b.Products.Count(p => p.IsActive)),
                _ => sortDesc ? brandsQuery.OrderByDescending(b => b.Id) : brandsQuery.OrderBy(b => b.Id)
            };

            var totalItems = await brandsQuery.CountAsync();

            var items = await brandsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .AsNoTracking()
                .ToListAsync();


            return (items, totalItems);
        }

        public async Task<(IEnumerable<Brand> Items, int TotalItems)> SearchAsync(string term, int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var normalizedTerm = term.Trim().ToLower();

            var query = _dbSet
                .Include(b => b.Products)
                .Where(b =>
                    EF.Functions.Like(b.Name, $"%{normalizedTerm}%") ||
                    (b.Description != null && EF.Functions.Like(b.Description, $"%{normalizedTerm}%")))
                .OrderBy(b => b.Name)
                .ThenBy(b => b.Id)
                .AsNoTracking();

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task<IEnumerable<string>> GetSearchRecommendationsAsync(string term, int size = 5)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Enumerable.Empty<string>();

            var normalizedTerm = term.Trim();

            return await _dbSet
                .Where(b => EF.Functions.Like(b.Name, $"%{normalizedTerm}%"))
                .OrderBy(b => b.Name)
                .Select(b => b.Name)
                .Distinct()
                .Take(size)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Brand>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idSet = ids.Distinct().ToList();
            if (!idSet.Any()) return Enumerable.Empty<Brand>();

            return await _dbSet
                .Include(b => b.Products)
                .Where(b => idSet.Contains(b.Id))
                .AsNoTracking()
                .ToListAsync();
        }

    }
}