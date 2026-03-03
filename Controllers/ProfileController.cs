using ECommerce.DTOs;
using ECommerce.DTOs.Profile;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{

    [ApiController]
    [Route("api/profile")]
    [Authorize(Roles = "Admin,User")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }


        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _profileService.GetProfileAsync(userId);
            return Ok(response);
        }


        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetUserById([FromRoute] string userId)
        {
            var response = await _profileService.GetUserByIdAsync(userId);
            return Ok(response);
        }


        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _profileService.UpdateProfileAsync(userId, dto);
            return Ok(response);
        }


        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _profileService.ChangePasswordAsync(userId, dto);
            return Ok(response);
        }

      
        [HttpPost("upload-image")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse.ErrorResponse("No file uploaded."));

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(ApiResponse.ErrorResponse("Invalid file type. Only images are allowed."));

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(ApiResponse.ErrorResponse("File size must be less than 5MB."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var result = await _profileService.UploadProfileImageAsync(userId, file);
            var imageUrl = result.Data?.ImageUrl ?? string.Empty;
            return Ok(ApiResponse<string>.Success(imageUrl, "Profile image uploaded successfully."));
        }

        /// <summary>
        /// Delete profile image
        /// </summary>
        [HttpDelete("delete-image")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _profileService.DeleteProfileImageAsync(userId);
            return Ok(response);
        }


        [HttpDelete("delete-account")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _profileService.DeleteAccountAsync(userId);
            return Ok(response);
        }
    }
}
