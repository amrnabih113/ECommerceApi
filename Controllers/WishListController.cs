using ECommerce.DTOs;
using ECommerce.DTOs.WishLists;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(Roles = "User,Admin")]
    [EnableRateLimiting("general")]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [HttpPost("products/{productId:int}/wishlist")]
        [ProducesResponseType(typeof(ApiResponse<WishListItemDto>), 200)]
        public async Task<IActionResult> AddToWishList([FromRoute] int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _wishListService.AddToWishListAsync(userId, productId);
            return Ok(response);
        }

        [HttpDelete("products/{productId:int}/wishlist")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> RemoveFromWishList([FromRoute] int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _wishListService.RemoveFromWishListAsync(userId, productId);
            return Ok(response);
        }

        [HttpGet("wishlist")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<WishListItemDto>>), 200)]
        public async Task<IActionResult> GetMyWishList(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _wishListService.GetUserWishListAsync(userId, page, pageSize);
            return Ok(response);
        }

        [HttpGet("products/{productId:int}/wishlist/check")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> CheckIfInWishList([FromRoute] int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _wishListService.IsProductInWishListAsync(userId, productId);
            return Ok(response);
        }
    }
}
