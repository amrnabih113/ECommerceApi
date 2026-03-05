using ECommerce.DTOs;
using ECommerce.DTOs.Banners;

namespace ECommerce.Interfaces.Services
{
    public interface IBannersService
    {
        Task<ApiResponse<BannerDto>> CreateAsync(BannerCreateDto dto);
        Task<ApiResponse<PageResult<BannerDto>>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<ApiResponse<BannerDto>> GetByIdAsync(int id);
        Task<ApiResponse<BannerDto>> UpdateAsync(int id, BannerUpdateDto dto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<BannerDto>>> GetActiveBannersAsync();
    }
}
