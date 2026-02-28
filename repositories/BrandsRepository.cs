using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;

namespace ECommerce.Repositories
{
    public class BrandsRepository : BaseRepository<Brand>, IBrandsRepository
    {
        public BrandsRepository(AppDbContext context) : base(context)
        {

        }
    }
}