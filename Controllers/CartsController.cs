using ECommerce.DTOs;
using ECommerce.DTOs.Carts;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general")]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartsService;

        public CartsController(ICartsService cartsService)
        {
            _cartsService = cartsService;
        }

        // User Endpoints
        [HttpGet("api/cart")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), 200)]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _cartsService.GetByUserIdAsync(userId);
            return Ok(response);
        }

        [HttpGet("api/cart/summary")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse<CartSummaryDto>), 200)]
        public async Task<IActionResult> GetCartSummary()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _cartsService.GetCartSummaryAsync(userId);
            return Ok(response);
        }

        [HttpDelete("api/cart")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _cartsService.ClearCartAsync(userId);
            return Ok(response);
        }

        // Admin Endpoints
        [HttpGet("api/admin/carts")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CartDto>>), 200)]
        public async Task<IActionResult> GetAllCarts(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var response = await _cartsService.GetAllAsync(page, pageSize);
            return Ok(response);
        }

        [HttpGet("api/admin/carts/{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), 200)]
        public async Task<IActionResult> GetCartById([FromRoute] int id)
        {
            var response = await _cartsService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("api/admin/carts/user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), 200)]
        public async Task<IActionResult> GetCartByUserId([FromRoute] string userId)
        {
            var response = await _cartsService.GetByUserIdAsync(userId);
            return Ok(response);
        }

        [HttpDelete("api/admin/carts/{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteCart([FromRoute] int id)
        {
            var response = await _cartsService.DeleteAsync(id);
            return Ok(response);
        }
    }
}
