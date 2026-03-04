using ECommerce.DTOs;
using ECommerce.DTOs.Coupons;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{

    [ApiController]
    [Route("api/coupons")]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponsService _couponsService;

        public CouponsController(ICouponsService couponsService)
        {
            _couponsService = couponsService;
        }


        /// Get all coupons (Admin only)

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CouponDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _couponsService.GetAllAsync(page, pageSize);
            return Ok(response);
        }


        /// Get active coupons

        [HttpGet("active")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CouponDto>>), 200)]
        public async Task<IActionResult> GetActiveCoupons([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _couponsService.GetActiveCouponsAsync(page, pageSize);
            return Ok(response);
        }


        /// Get coupon by ID

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CouponDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _couponsService.GetByIdAsync(id);
            return Ok(response);
        }


        /// Get coupon by code

        [HttpGet("code/{code}")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(ApiResponse<CouponDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetByCode(string code)
        {
            var response = await _couponsService.GetByCodeAsync(code);
            return Ok(response);
        }


        /// Validate a coupon code

        [HttpPost("validate")]
        [Authorize(Roles = "Admin,User")]
        [ProducesResponseType(typeof(ApiResponse<CouponValidationResult>), 200)]
        public async Task<IActionResult> Validate([FromBody] ValidateCouponDto dto)
        {
            var response = await _couponsService.ValidateCouponAsync(dto.Code, dto.OrderAmount);
            return Ok(response);
        }


        /// Create a new coupon (Admin only)

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CouponDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var response = await _couponsService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }


        /// Update a coupon (Admin only)

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CouponDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var response = await _couponsService.UpdateAsync(id, dto);
            return Ok(response);
        }


        /// Delete a coupon (Admin only)

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 204)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _couponsService.DeleteAsync(id);
            if (!response.Success)
                return NotFound(response);

            return NoContent();
        }

        /// <summary>
        /// Get current user's available coupons
        /// </summary>
        [HttpGet("my-coupons")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CouponDto>>), 200)]
        public async Task<IActionResult> GetMyCoupons([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _couponsService.GetUserCouponsAsync(userId, page, pageSize);
            return Ok(response);
        }

        /// <summary>
        /// Assign a coupon to a user (Admin only)
        /// </summary>
        [HttpPost("{couponId:int}/assign-user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> AssignCouponToUser(int couponId, string userId)
        {
            var response = await _couponsService.AssignCouponToUserAsync(couponId, userId);
            return Ok(response);
        }

        /// <summary>
        /// Remove coupon access from a user (Admin only)
        /// </summary>
        [HttpDelete("{couponId:int}/remove-user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> RemoveCouponFromUser(int couponId, string userId)
        {
            var response = await _couponsService.RemoveCouponFromUserAsync(couponId, userId);
            return Ok(response);
        }

        /// <summary>
        /// Bulk assign a coupon to multiple users (Admin only)
        /// </summary>
        [HttpPost("{couponId:int}/assign-users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> BulkAssignCoupon(int couponId, [FromBody] BulkAssignCouponDto dto)
        {
            var response = await _couponsService.BulkAssignCouponAsync(couponId, dto.UserIds);
            return Ok(response);
        }

        /// <summary>
        /// Get all users assigned to a coupon (Admin only)
        /// </summary>
        [HttpGet("{couponId:int}/users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<UserCouponInfoDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetCouponUsers(int couponId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _couponsService.GetCouponUsersAsync(couponId, page, pageSize);
            return Ok(response);
        }
    }
}
