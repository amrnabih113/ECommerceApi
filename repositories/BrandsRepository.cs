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

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                brandsQuery = brandsQuery.Where(b =>
                    b.Name.ToLower().Contains(search) ||
                    (b.Description != null && b.Description.ToLower().Contains(search)));
            }

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

    }
}