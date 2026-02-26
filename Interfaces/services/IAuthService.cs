
using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.Models;

namespace ECommerce.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResponse> VerifyOtpAsync(VerifyOtpDto dto);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto);
        Task SendOtpAsync(ApplicationUser user);
    }
}