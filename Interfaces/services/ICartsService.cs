using ECommerce.DTOs;
using ECommerce.DTOs.Carts;

namespace ECommerce.Interfaces.Services
{
    public interface ICartsService
    {
        // User endpoints
        Task<ApiResponse<CartDto>> GetByUserIdAsync(string userId);
        Task<ApiResponse> ClearCartAsync(string userId);

        // Admin endpoints
        Task<ApiResponse<PageResult<CartDto>>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<ApiResponse<CartDto>> GetByIdAsync(int id);
        Task<ApiResponse> DeleteAsync(int id);
    }
}
