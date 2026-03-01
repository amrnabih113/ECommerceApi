using ECommerce.DTOs;
using ECommerce.DTOs.Carts;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Authorize]
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

        [HttpDelete("api/cart/clear")]
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
