using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IUserCouponsRepository
    {
        /// <summary>
        /// Check if user has access to a coupon
        /// </summary>
        Task<bool> UserHasAccessAsync(string userId, int couponId);

        /// <summary>
        /// Get coupons available to a specific user
        /// </summary>
        Task<(IEnumerable<Coupon> Items, int TotalCount)> GetUserCouponsAsync(string userId, int page, int pageSize);

        /// <summary>
        /// Get all users assigned to a coupon
        /// </summary>
        Task<(IEnumerable<UserCoupon> Items, int TotalCount)> GetCouponUsersAsync(int couponId, int page, int pageSize);

        /// <summary>
        /// Assign a coupon to a user
        /// </summary>
        Task<UserCoupon> AssignCouponToUserAsync(string userId, int couponId);

        /// <summary>
        /// Remove coupon access from a user
        /// </summary>
        Task<bool> RemoveCouponFromUserAsync(string userId, int couponId);

        /// <summary>
        /// Revoke all users from a coupon
        /// </summary>
        Task<int> RemoveAllUsersFromCouponAsync(int couponId);

        /// <summary>
        /// Bulk assign coupon to multiple users
        /// </summary>
        Task<int> BulkAssignCouponAsync(int couponId, IEnumerable<string> userIds);

        /// <summary>
        /// Get or create user coupon relationship
        /// </summary>
        Task<UserCoupon> GetOrCreateUserCouponAsync(string userId, int couponId);

        /// <summary>
        /// Increment user usage count
        /// </summary>
        Task<bool> IncrementUserUsageCountAsync(string userId, int couponId);

        /// <summary>
        /// Get user coupon by ID
        /// </summary>
        Task<UserCoupon?> GetByIdAsync(int userCouponId);
    }
}
