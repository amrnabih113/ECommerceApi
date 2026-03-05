using AutoMapper;
using ECommerce.core.utils;
using ECommerce.DTOs;
using ECommerce.DTOs.Coupons;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class CouponsService : ICouponsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CouponsService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PageResult<CouponDto>>> GetAllAsync(int page, int pageSize)
        {
            var (items, totalItems) = await _unitOfWork.Coupons.GetPagedAsync(page, pageSize);
            var couponDtos = _mapper.Map<List<CouponDto>>(items);

            var pageResult = new PageResult<CouponDto>
            {
                Items = couponDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return ApiResponse<PageResult<CouponDto>>.SuccessResponse(pageResult, "Coupons retrieved successfully.");
        }

        public async Task<ApiResponse<PageResult<CouponDto>>> GetActiveCouponsAsync(int page, int pageSize)
        {
            var (items, totalItems) = await _unitOfWork.Coupons.GetActiveCouponsAsync(page, pageSize);
            var couponDtos = _mapper.Map<List<CouponDto>>(items);

            var pageResult = new PageResult<CouponDto>
            {
                Items = couponDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return ApiResponse<PageResult<CouponDto>>.SuccessResponse(pageResult, "Active coupons retrieved successfully.");
        }

        public async Task<ApiResponse<CouponDto>> GetByIdAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
                return ApiResponse<CouponDto>.Error("Coupon not found.");

            var couponDto = _mapper.Map<CouponDto>(coupon);
            return ApiResponse<CouponDto>.SuccessResponse(couponDto, "Coupon retrieved successfully.");
        }

        public async Task<ApiResponse<CouponDto>> GetByCodeAsync(string code)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);
            if (coupon == null)
                return ApiResponse<CouponDto>.Error("Coupon not found.");

            var couponDto = _mapper.Map<CouponDto>(coupon);
            return ApiResponse<CouponDto>.SuccessResponse(couponDto, "Coupon retrieved successfully.");
        }

        public async Task<ApiResponse<CouponDto>> CreateAsync(CreateCouponDto dto)
        {
            // Check if code already exists
            if (await _unitOfWork.Coupons.CodeExistsAsync(dto.Code))
                return ApiResponse<CouponDto>.Error("Coupon code already exists.");

            // Validate dates
            if (dto.ValidFrom >= dto.ValidUntil)
                return ApiResponse<CouponDto>.Error("Valid from date must be before valid until date.");

            // Validate discount value for percentage
            if (dto.DiscountType == DiscountType.Percentage && (dto.DiscountValue < 0 || dto.DiscountValue > 100))
                return ApiResponse<CouponDto>.Error("Percentage discount must be between 0 and 100.");

            var coupon = _mapper.Map<Coupon>(dto);
            coupon.UsedCount = 0;
            coupon.IsActive = true;

            var createdCoupon = await _unitOfWork.Coupons.AddAsync(coupon);
            await _unitOfWork.CompleteAsync();
            var couponDto = _mapper.Map<CouponDto>(createdCoupon);

            return ApiResponse<CouponDto>.SuccessResponse(couponDto, "Coupon created successfully.");
        }

        public async Task<ApiResponse<CouponDto>> UpdateAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
                return ApiResponse<CouponDto>.Error("Coupon not found.");

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(dto.Description))
                coupon.Description = dto.Description;

            if (dto.DiscountType.HasValue)
                coupon.DiscountType = dto.DiscountType.Value;

            if (dto.DiscountValue.HasValue)
            {
                if (coupon.DiscountType == DiscountType.Percentage && (dto.DiscountValue.Value < 0 || dto.DiscountValue.Value > 100))
                    return ApiResponse<CouponDto>.Error("Percentage discount must be between 0 and 100.");

                coupon.DiscountValue = dto.DiscountValue.Value;
            }

            if (dto.MinimumOrderAmount.HasValue)
                coupon.MinimumOrderAmount = dto.MinimumOrderAmount.Value;

            if (dto.MaximumDiscountAmount.HasValue)
                coupon.MaximumDiscountAmount = dto.MaximumDiscountAmount.Value;

            if (dto.ValidFrom.HasValue)
                coupon.ValidFrom = dto.ValidFrom.Value;

            if (dto.ValidUntil.HasValue)
                coupon.ValidUntil = dto.ValidUntil.Value;

            if (dto.IsActive.HasValue)
                coupon.IsActive = dto.IsActive.Value;

            if (dto.UsageLimit.HasValue)
                coupon.UsageLimit = dto.UsageLimit.Value;

            // Validate dates
            if (coupon.ValidFrom >= coupon.ValidUntil)
                return ApiResponse<CouponDto>.Error("Valid from date must be before valid until date.");

            await _unitOfWork.Coupons.UpdateAsync(coupon);
            await _unitOfWork.CompleteAsync();
            var couponDto = _mapper.Map<CouponDto>(coupon);

            return ApiResponse<CouponDto>.SuccessResponse(couponDto, "Coupon updated successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(id);
            if (coupon == null)
                return ApiResponse.ErrorResponse("Coupon not found.");

            await _unitOfWork.Coupons.DeleteAsync(coupon);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Coupon deleted successfully.");
        }



        /// <summary>
        /// Get coupons available to a specific user
        /// </summary>
        public async Task<ApiResponse<PageResult<CouponDto>>> GetUserCouponsAsync(string userId, int page, int pageSize)
        {
            var (items, totalItems) = await _unitOfWork.UserCoupons.GetUserCouponsAsync(userId, page, pageSize);
            var couponDtos = _mapper.Map<List<CouponDto>>(items);

            var pageResult = new PageResult<CouponDto>
            {
                Items = couponDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return ApiResponse<PageResult<CouponDto>>.SuccessResponse(pageResult, "User coupons retrieved successfully.");
        }

        public async Task<ApiResponse<CouponValidationResult>> ValidateCouponAsync(string code, decimal orderAmount, string? userId = null)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code);
            var result = new CouponValidationResult();

            if (coupon == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid coupon code.";
                return ApiResponse<CouponValidationResult>.SuccessResponse(result);
            }

            // Check if user has access to this coupon (if userId provided)
            if (!string.IsNullOrEmpty(userId))
            {
                var hasAccess = await _unitOfWork.UserCoupons.UserHasAccessAsync(userId, coupon.Id);
                if (!hasAccess)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "You don't have access to this coupon.";
                    return ApiResponse<CouponValidationResult>.SuccessResponse(result);
                }
            }

            if (!coupon.IsActive)
            {
                result.IsValid = false;
                result.ErrorMessage = "This coupon is no longer active.";
                return ApiResponse<CouponValidationResult>.SuccessResponse(result);
            }

            if (DateTime.UtcNow < coupon.ValidFrom)
            {
                result.IsValid = false;
                result.ErrorMessage = "This coupon is not yet valid.";
                return ApiResponse<CouponValidationResult>.SuccessResponse(result);
            }

            if (DateTime.UtcNow > coupon.ValidUntil)
            {
                result.IsValid = false;
                result.ErrorMessage = "This coupon has expired.";
                return ApiResponse<CouponValidationResult>.SuccessResponse(result);
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                result.IsValid = false;
                result.ErrorMessage = "This coupon has reached its usage limit.";
                return ApiResponse<CouponValidationResult>.SuccessResponse(result);
            }

            if (coupon.MinimumOrderAmount.HasValue && orderAmount < coupon.MinimumOrderAmount.Value)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Minimum order amount of ${coupon.MinimumOrderAmount.Value:F2} is required.";
                return ApiResponse<CouponValidationResult>.SuccessResponse(result);
            }

            // Calculate discount
            decimal discountAmount = 0;
            if (coupon.DiscountType == DiscountType.Percentage)
            {
                discountAmount = orderAmount * (coupon.DiscountValue / 100m);

                if (coupon.MaximumDiscountAmount.HasValue && discountAmount > coupon.MaximumDiscountAmount.Value)
                    discountAmount = coupon.MaximumDiscountAmount.Value;
            }
            else // FixedAmount
            {
                discountAmount = coupon.DiscountValue;
            }

            // Ensure discount doesn't exceed order amount
            if (discountAmount > orderAmount)
                discountAmount = orderAmount;

            result.IsValid = true;
            result.DiscountAmount = discountAmount;
            result.Coupon = _mapper.Map<CouponDto>(coupon);

            return ApiResponse<CouponValidationResult>.SuccessResponse(result, "Coupon is valid.");
        }

        /// <summary>
        /// Assign a coupon to a user
        /// </summary>
        public async Task<ApiResponse> AssignCouponToUserAsync(int couponId, string userId)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(couponId);
            if (coupon == null)
                return ApiResponse.ErrorResponse("Coupon not found.");

            var userCoupon = await _unitOfWork.UserCoupons.AssignCouponToUserAsync(userId, couponId);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse($"Coupon assigned to user successfully.");
        }

        /// <summary>
        /// Remove coupon access from a user
        /// </summary>
        public async Task<ApiResponse> RemoveCouponFromUserAsync(int couponId, string userId)
        {
            var removed = await _unitOfWork.UserCoupons.RemoveCouponFromUserAsync(userId, couponId);
            if (!removed)
                return ApiResponse.ErrorResponse("User-coupon assignment not found.");

            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Coupon access removed from user.");
        }

        /// <summary>
        /// Bulk assign a coupon to multiple users
        /// </summary>
        public async Task<ApiResponse> BulkAssignCouponAsync(int couponId, IEnumerable<string> userIds)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(couponId);
            if (coupon == null)
                return ApiResponse.ErrorResponse("Coupon not found.");

            var count = await _unitOfWork.UserCoupons.BulkAssignCouponAsync(couponId, userIds);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse($"Coupon assigned to {count} users successfully.");
        }

        /// <summary>
        /// Get all users assigned to a coupon
        /// </summary>
        public async Task<ApiResponse<PageResult<UserCouponInfoDto>>> GetCouponUsersAsync(int couponId, int page, int pageSize)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(couponId);
            if (coupon == null)
                return ApiResponse<PageResult<UserCouponInfoDto>>.Error("Coupon not found.");

            var (userCoupons, totalCount) = await _unitOfWork.UserCoupons.GetCouponUsersAsync(couponId, page, pageSize);

            var userCouponInfos = userCoupons.Select(uc => new UserCouponInfoDto
            {
                Id = uc.Id,
                UserId = uc.UserId,
                UserName = uc.User?.UserName ?? string.Empty,
                UserEmail = uc.User?.Email,
                CanUse = uc.CanUse,
                UserUsageCount = uc.UserUsageCount,
                AssignedAt = uc.AssignedAt
            }).ToList();

            var pageResult = new PageResult<UserCouponInfoDto>
            {
                Items = userCouponInfos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            return ApiResponse<PageResult<UserCouponInfoDto>>.SuccessResponse(pageResult, "Coupon users retrieved successfully.");
        }
    }
}
