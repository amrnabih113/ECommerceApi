using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Data;
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
            var skip = (page - 1) * pageSize;
            var items = await _dbSet
                .Include(c => c.Products)
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var totalItems = await _dbSet.CountAsync();

            return (items, totalItems);
        }

    }
}