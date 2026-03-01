using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;

namespace ECommerce.Repositories
{
    public class ProductVariantsRepository : BaseRepository<ProductVariant>, IProductVariantsRepository
    {
        public ProductVariantsRepository(AppDbContext context) : base(context) { }
    }
}
