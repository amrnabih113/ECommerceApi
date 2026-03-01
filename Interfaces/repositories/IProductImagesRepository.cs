using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IProductImagesRepository : IBaseRepository<ProductImage>
    {
        Task<(IEnumerable<ProductImage> Items, int TotalItems)> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10);
        Task<IEnumerable<ProductImage>> GetAllByProductIdAsync(int productId);
        Task<ProductImage?> GetByProductIdAndImageIdAsync(int productId, int imageId);
    }
}