using ECommerce.DTOs;
using ECommerce.DTOs.Products;

namespace ECommerce.Interfaces.Services
{
    public interface IProductVariantsService
    {
        Task<ApiResponse<ProductVariantDto>> CreateAsync(int productId, ProductVariantCreateDto dto);
        Task<ApiResponse<ProductVariantDto>> UpdateAsync(int productId, int variantId, ProductVariantUpdateDto dto);
        Task<ApiResponse> DeleteAsync(int productId, int variantId);
        Task<ApiResponse<PageResult<ProductVariantDto>>> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10);
        Task<ApiResponse<ProductVariantDto>> GetByIdAsync(int productId, int variantId);
    }
}
