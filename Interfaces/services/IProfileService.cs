using ECommerce.DTOs;
using ECommerce.DTOs.Profile;

namespace ECommerce.Interfaces.Services
{
    public interface IProfileService
    {
        Task<ApiResponse<ProfileDto>> GetProfileAsync(string userId);
        Task<ApiResponse<ProfileDto>> GetUserByIdAsync(string userId);
        Task<ApiResponse<ProfileDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<ApiResponse<ProfileDto>> UploadProfileImageAsync(string userId, IFormFile file);
        Task<ApiResponse> DeleteProfileImageAsync(string userId);
        Task<ApiResponse> DeleteAccountAsync(string userId);
    }
}
