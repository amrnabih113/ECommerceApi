using ECommerce.DTOs;
using ECommerce.DTOs.Brands;

namespace ECommerce.Interfaces.Services
{
    public interface IBrandsService
    {
        Task<ApiResponse<PageResult<BrandDto>>> GetAllAsync(BrandQueryDto query);
        Task<ApiResponse<BrandDto>> GetByIdAsync(int id);
        Task<ApiResponse<BrandDto>> CreateAsync(BrandCreateDto dto);
        Task<ApiResponse<BrandDto>> UpdateAsync(int id, BrandUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}