using ECommerce.DTOs;
using ECommerce.DTOs.Products;

namespace ECommerce.Interfaces.Services
{

    public interface IProductsService
    {
        Task<ApiResponse<PageResult<ProductDto>>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<ApiResponse<ProductDto>> GetByIdAsync(int id);
        Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateDto dto);
        Task<ApiResponse<ProductDto>> UpdateAsync(int id, ProductUpdateDto dto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse<PageResult<ProductDto>>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10);
        Task<ApiResponse<PageResult<ProductDto>>> GetByBrandAsync(int brandId, int page = 1, int pageSize = 10);
    }
}
