using ECommerce.DTOs;
using ECommerce.DTOs.Banners;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.Controllers
{
    /// <summary>
    /// Banners management endpoints for admin and users
    /// </summary>
    [ApiController]
    [Route("api/banners")]
    [EnableRateLimiting("general")]
    public class BannersController : ControllerBase
    {
        private readonly IBannersService _bannersService;

        public BannersController(IBannersService bannersService)
        {
            _bannersService = bannersService;
        }

        /// <summary>
        /// Get all banners with pagination (Admin only)
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>Paginated list of banners</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<BannerDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var response = await _bannersService.GetAllAsync(page, pageSize);
            return Ok(response);
        }

        /// <summary>
        /// Get active banners for display (Public - no authentication required)
        /// </summary>
        /// <returns>List of active banners ordered by display order</returns>
        [HttpGet("active")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BannerDto>>), 200)]
        public async Task<IActionResult> GetActiveBanners()
        {
            var response = await _bannersService.GetActiveBannersAsync();
            return Ok(response);
        }

        /// <summary>
        /// Get banner by ID (Public - no authentication required)
        /// </summary>
        /// <param name="id">Banner ID</param>
        /// <returns>Banner details</returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<BannerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await _bannersService.GetByIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Create a new banner (Admin only)
        /// </summary>
        /// <param name="dto">Banner creation data</param>
        /// <returns>Created banner</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<BannerDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Create([FromBody] BannerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _bannersService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }

        /// <summary>
        /// Update a banner (Admin only)
        /// </summary>
        /// <param name="id">Banner ID</param>
        /// <param name="dto">Banner update data</param>
        /// <returns>Updated banner</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<BannerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BannerUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _bannersService.UpdateAsync(id, dto);
            return Ok(response);
        }

        /// <summary>
        /// Delete a banner (Admin only)
        /// </summary>
        /// <param name="id">Banner ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var response = await _bannersService.DeleteAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Toggle banner active status (Admin only)
        /// </summary>
        /// <param name="id">Banner ID</param>
        /// <returns>Updated banner</returns>
        [HttpPatch("{id:int}/toggle")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<BannerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> ToggleActive([FromRoute] int id)
        {
            var banner = await _bannersService.GetByIdAsync(id);
            if (banner.Data == null)
                return NotFound(ApiResponse.ErrorResponse("Banner not found."));

            var updateDto = new BannerUpdateDto { IsActive = !banner.Data.IsActive };
            var response = await _bannersService.UpdateAsync(id, updateDto);
            return Ok(response);
        }
    }
}
