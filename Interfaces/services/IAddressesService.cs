using ECommerce.DTOs;
using ECommerce.DTOs.Orders;

namespace ECommerce.Interfaces.Services
{
    public interface IAddressesService
    {
        Task<ApiResponse<List<AddressDto>>> GetMyAddressesAsync(string userId);
        Task<ApiResponse<AddressDto>> GetByIdAsync(int id, string userId);
        Task<ApiResponse<AddressDto>> CreateAsync(CreateAddressDto dto, string userId);
        Task<ApiResponse<AddressDto>> UpdateAsync(int id, UpdateAddressDto dto, string userId);
        Task<ApiResponse> DeleteAsync(int id, string userId);
    }
}
