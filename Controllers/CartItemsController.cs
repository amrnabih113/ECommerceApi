using ECommerce.DTOs;
using ECommerce.DTOs.CartItems;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Authorize]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        private readonly ICartsService _cartsService;

        public CartItemsController(ICartItemsService cartItemsService, ICartsService cartsService)
        {
            _cartItemsService = cartItemsService;
            _cartsService = cartsService;
        }

        // User Endpoints
        [HttpPost("api/cart/items")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse<CartItemDto>), 200)]
        public async Task<IActionResult> AddToCart([FromBody] CartItemCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _cartItemsService.AddToCartAsync(userId, dto);
            return Ok(response);
        }

        [HttpPut("api/cart/items/{itemId:int}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse<CartItemDto>), 200)]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int itemId, [FromBody] CartItemUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _cartItemsService.UpdateQuantityAsync(userId, itemId, dto.Quantity);
            return Ok(response);
        }

        [HttpDelete("api/cart/items/{itemId:int}")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> RemoveFromCart([FromRoute] int itemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _cartItemsService.RemoveFromCartAsync(userId, itemId);
            return Ok(response);
        }

    }
}
