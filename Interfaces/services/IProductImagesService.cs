using ECommerce.DTOs;
using ECommerce.DTOs.Products;

namespace ECommerce.Interfaces.Services
{
    public interface IProductImagesService
    {
        Task<ApiResponse<PageResult<ProductImageDto>>> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10);
        Task<ApiResponse<ProductImageDto>> GetByIdAsync(int productId, int imageId);
        Task<ApiResponse<ProductImageDto>> UploadAsync(int productId, ProductImageUploadDto dto);
        Task<ApiResponse<ProductImageDto>> UpdateAsync(int productId, int imageId, ProductImageUpdateDto dto);
        Task<ApiResponse> DeleteAsync(int productId, int imageId);
    }
}