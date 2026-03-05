using ECommerce.DTOs;
using ECommerce.DTOs.Reviews;
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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;

        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }

        [HttpGet("products/{productId:int}/reviews")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ReviewDto>>), 200)]
        public async Task<IActionResult> GetByProduct([FromRoute] int productId, int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var response = await _reviewsService.GetByProductIdAsync(productId, page, pageSize);
            return Ok(response);
        }

        [HttpPost("products/{productId:int}/reviews")]
        [ProducesResponseType(typeof(ApiResponse<ReviewDto>), 200)]
        public async Task<IActionResult> Create([FromRoute] int productId, [FromBody] ReviewCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _reviewsService.CreateAsync(userId, productId, dto);
            return Ok(response);
        }

        [HttpPut("reviews/{reviewId:int}")]
        [ProducesResponseType(typeof(ApiResponse<ReviewDto>), 200)]
        public async Task<IActionResult> Update([FromRoute] int reviewId, [FromBody] ReviewUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _reviewsService.UpdateAsync(userId, reviewId, dto);
            return Ok(response);
        }

        [HttpDelete("reviews/{reviewId:int}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Delete([FromRoute] int reviewId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _reviewsService.DeleteAsync(userId, reviewId);
            return Ok(response);
        }

        [HttpDelete("admin/reviews/{reviewId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> DeleteAsAdmin([FromRoute] int reviewId)
        {
            var response = await _reviewsService.DeleteAsync(reviewId);
            return Ok(response);
        }
    }
}