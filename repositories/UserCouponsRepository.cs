using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Repositories
{
    public class UserCouponsRepository : IUserCouponsRepository
    {
        private readonly AppDbContext _context;

        public UserCouponsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasAccessAsync(string userId, int couponId)
        {
            return await _context.Set<UserCoupon>()
                .AnyAsync(uc => uc.UserId == userId && uc.CouponId == couponId && uc.CanUse);
        }

        public async Task<(IEnumerable<Coupon> Items, int TotalCount)> GetUserCouponsAsync(string userId, int page, int pageSize)
        {
            var query = _context.Set<UserCoupon>()
                .Where(uc => uc.UserId == userId && uc.CanUse)
                .Include(uc => uc.Coupon)
                .Where(uc => uc.Coupon.IsActive && uc.Coupon.ValidFrom <= DateTime.UtcNow && uc.Coupon.ValidUntil >= DateTime.UtcNow)
                .Select(uc => uc.Coupon);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<UserCoupon> Items, int TotalCount)> GetCouponUsersAsync(int couponId, int page, int pageSize)
        {
            var query = _context.Set<UserCoupon>()
                .Where(uc => uc.CouponId == couponId)
                .Include(uc => uc.User);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<UserCoupon> AssignCouponToUserAsync(string userId, int couponId)
        {
            var existing = await _context.Set<UserCoupon>()
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId);

            if (existing != null)
            {
                existing.CanUse = true;
                existing.AssignedAt = DateTime.UtcNow;
                _context.Set<UserCoupon>().Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }

            var userCoupon = new UserCoupon
            {
                UserId = userId,
                CouponId = couponId,
                CanUse = true,
                UserUsageCount = 0,
                AssignedAt = DateTime.UtcNow
            };

            _context.Set<UserCoupon>().Add(userCoupon);
            await _context.SaveChangesAsync();
            return userCoupon;
        }

        public async Task<bool> RemoveCouponFromUserAsync(string userId, int couponId)
        {
            var userCoupon = await _context.Set<UserCoupon>()
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId);

            if (userCoupon == null)
                return false;

            _context.Set<UserCoupon>().Remove(userCoupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> RemoveAllUsersFromCouponAsync(int couponId)
        {
            var userCoupons = await _context.Set<UserCoupon>()
                .Where(uc => uc.CouponId == couponId)
                .ToListAsync();

            _context.Set<UserCoupon>().RemoveRange(userCoupons);
            await _context.SaveChangesAsync();
            return userCoupons.Count;
        }

        public async Task<int> BulkAssignCouponAsync(int couponId, IEnumerable<string> userIds)
        {
            var userIdsList = userIds.ToList();
            var existingAssignments = await _context.Set<UserCoupon>()
                .Where(uc => uc.CouponId == couponId && userIdsList.Contains(uc.UserId))
                .ToListAsync();

            var existingUserIds = existingAssignments.Select(uc => uc.UserId).ToList();

            // Re-enable existing assignments
            foreach (var existing in existingAssignments.Where(e => !e.CanUse))
            {
                existing.CanUse = true;
                existing.AssignedAt = DateTime.UtcNow;
            }

            // Create new assignments for users not yet assigned
            var newUserIds = userIdsList.Except(existingUserIds).ToList();
            var newAssignments = newUserIds.Select(userId => new UserCoupon
            {
                UserId = userId,
                CouponId = couponId,
                CanUse = true,
                UserUsageCount = 0,
                AssignedAt = DateTime.UtcNow
            }).ToList();

            _context.Set<UserCoupon>().AddRange(newAssignments);
            await _context.SaveChangesAsync();

            return newAssignments.Count;
        }

        public async Task<UserCoupon> GetOrCreateUserCouponAsync(string userId, int couponId)
        {
            var userCoupon = await _context.Set<UserCoupon>()
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId);

            if (userCoupon != null)
                return userCoupon;

            userCoupon = new UserCoupon
            {
                UserId = userId,
                CouponId = couponId,
                CanUse = true,
                UserUsageCount = 0,
                AssignedAt = DateTime.UtcNow
            };

            _context.Set<UserCoupon>().Add(userCoupon);
            await _context.SaveChangesAsync();
            return userCoupon;
        }

        public async Task<bool> IncrementUserUsageCountAsync(string userId, int couponId)
        {
            var userCoupon = await _context.Set<UserCoupon>()
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId);

            if (userCoupon == null)
                return false;

            userCoupon.UserUsageCount++;
            _context.Set<UserCoupon>().Update(userCoupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserCoupon?> GetByIdAsync(int userCouponId)
        {
            return await _context.Set<UserCoupon>()
                .Include(uc => uc.User)
                .Include(uc => uc.Coupon)
                .FirstOrDefaultAsync(uc => uc.Id == userCouponId);
        }
    }
}
