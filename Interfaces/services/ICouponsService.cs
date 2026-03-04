using ECommerce.DTOs;
using ECommerce.DTOs.Coupons;

namespace ECommerce.Interfaces.Services
{
    public interface ICouponsService
    {
        Task<ApiResponse<PageResult<CouponDto>>> GetAllAsync(int page, int pageSize);
        Task<ApiResponse<PageResult<CouponDto>>> GetActiveCouponsAsync(int page, int pageSize);
        Task<ApiResponse<PageResult<CouponDto>>> GetUserCouponsAsync(string userId, int page, int pageSize);
        Task<ApiResponse<CouponDto>> GetByIdAsync(int id);
        Task<ApiResponse<CouponDto>> GetByCodeAsync(string code);
        Task<ApiResponse<CouponDto>> CreateAsync(CreateCouponDto dto);
        Task<ApiResponse<CouponDto>> UpdateAsync(int id, UpdateCouponDto dto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse<CouponValidationResult>> ValidateCouponAsync(string code, decimal orderAmount, string? userId = null);

        Task<ApiResponse> AssignCouponToUserAsync(int couponId, string userId);
        Task<ApiResponse> RemoveCouponFromUserAsync(int couponId, string userId);
        Task<ApiResponse> BulkAssignCouponAsync(int couponId, IEnumerable<string> userIds);
        Task<ApiResponse<PageResult<UserCouponInfoDto>>> GetCouponUsersAsync(int couponId, int page, int pageSize);
    }
}
