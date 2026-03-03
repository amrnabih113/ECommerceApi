using ECommerce.DTOs;
using ECommerce.DTOs.Products;

namespace ECommerce.Interfaces.Services
{

    public interface IProductsService
    {
        Task<ApiResponse<PageResult<ProductDto>>> GetAllAsync(ProductQueryDto query, string? userId = null);
        Task<ApiResponse<ProductDetailsDto>> GetByIdAsync(int id, string? userId = null);
        Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateDto dto);
        Task<ApiResponse<ProductDetailsDto>> UpdateAsync(int id, ProductUpdateDto dto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse<PageResult<ProductDto>>> GetByCategoryAsync(int categoryId, ProductQueryDto query, string? userId = null);
        Task<ApiResponse<PageResult<ProductDto>>> GetByBrandAsync(int brandId, ProductQueryDto query, string? userId = null);
    }
}
