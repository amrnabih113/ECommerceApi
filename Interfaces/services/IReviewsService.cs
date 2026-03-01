using ECommerce.DTOs;
using ECommerce.DTOs.Reviews;

namespace ECommerce.Interfaces.Services
{
    public interface IReviewsService
    {
        Task<ApiResponse<PageResult<ReviewDto>>> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10);
        Task<ApiResponse<ReviewDto>> CreateAsync(string userId, int productId, ReviewCreateDto dto);
        Task<ApiResponse<ReviewDto>> UpdateAsync(string userId, int reviewId, ReviewUpdateDto dto);
        Task<ApiResponse> DeleteAsync(string userId, int reviewId);
        Task<ApiResponse> DeleteAsync(int reviewId);
    }
}