using ECommerce.DTOs;
using ECommerce.DTOs.Brands;

namespace ECommerce.Interfaces.Services
{
    public interface IBrandsService
    {
        Task<IEnumerable<BrandDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<BrandDto> GetByIdAsync(int id);
        Task<BrandDto> CreateAsync(BrandCreateDto dto);
        Task<BrandDto> UpdateAsync(int id, BrandUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}