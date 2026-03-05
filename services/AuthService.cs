using ECommerce.core.Exceptions;
using ECommerce.core.utils;
using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshRepo,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshRepo = refreshRepo;
            _emailService = emailService;
            _logger = logger;
        }


        public async Task<ApiResponse> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Username ?? dto.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

            var roleResult = await _userManager.AddToRoleAsync(user, AppConstants.UserRole);
            if (!roleResult.Succeeded)
                throw new BadRequestException("Failed to assign role.");

            await SendOtpAsync(user);
            _logger.LogInformation("User registration created. UserId: {UserId}", user.Id);

            var response = new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email!
            };

            return ApiResponse<RegisterResponseDto>
                .SuccessResponse(response, "Registration successful. Please verify your email.");
        }


        public async Task<ApiResponse> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Failed login attempt for unknown email {Email}", dto.Email);
                throw new UnauthorizedException("Invalid Email or Password.");
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login blocked for unverified email. UserId: {UserId}", user.Id);
                await SendOtpAsync(user);
                throw new BadRequestException("Email is not verified. OTP sent.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Failed login attempt for user {UserId}", user.Id);
                throw new UnauthorizedException("Invalid Email or Password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles.ToList());

            var refreshPlain = _tokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(refreshPlain),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(15),
            };

            await _refreshRepo.AddAsync(refreshToken);
            await _refreshRepo.SaveAsync();

            _logger.LogInformation("User login succeeded. UserId: {UserId}", user.Id);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Address = user.Address,
                ImageUrl = user.ImageUrl,
                AccessToken = accessToken,
                RefreshToken = refreshPlain
            };

            return ApiResponse<LoginResponseDto>
                .SuccessResponse(response, "Login successful.");
        }


        public async Task<ApiResponse> VerifyAccountAsync(VerifyOtpDto dto)
        {
            var otpRecord = await _context
                .EmailOtps.Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User!.Email == dto.Email && o.Code == dto.Otp);

            if (otpRecord == null)
                throw new BadRequestException("Invalid OTP.");

            if (otpRecord.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("OTP expired.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new BadRequestException("User not found.");
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            _context.EmailOtps.Remove(otpRecord);
            await _context.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles.ToList());

            var refreshPlain = _tokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(refreshPlain),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(15),
            };

            await _refreshRepo.AddAsync(refreshToken);
            await _refreshRepo.SaveAsync();

            _logger.LogInformation("Account verified successfully. UserId: {UserId}", user.Id);

            var response = new VerifyOtpResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Address = user.Address,
                ImageUrl = user.ImageUrl,
                AccessToken = accessToken,
                RefreshToken = refreshPlain
            };
            return ApiResponse<VerifyOtpResponseDto>.SuccessResponse(response, "OTP verified successfully.");
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null)
            {
                await SendOtpAsync(user);
                _logger.LogInformation("Password reset OTP sent. UserId: {UserId}", user.Id);
            }

            // Always return success message to prevent email enumeration
            return ApiResponse.SuccessResponse("OTP sent successfully.");
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new BadRequestException("User not found.");

            var otpRecord = await _context.EmailOtps.FirstOrDefaultAsync(o =>
                o.Email == dto.Email && o.Code == dto.Otp
            );

            if (otpRecord == null)
                throw new BadRequestException("Invalid OTP.");

            if (otpRecord.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("OTP expired.");

            var removePass = await _userManager.RemovePasswordAsync(user);
            if (!removePass.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", removePass.Errors.Select(e => e.Description))
                );

            var addPass = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addPass.Succeeded)
                throw new BadRequestException(
                    string.Join(", ", addPass.Errors.Select(e => e.Description))
                );

            _context.EmailOtps.Remove(otpRecord);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password reset succeeded. UserId: {UserId}", user.Id);

            return ApiResponse.SuccessResponse("Password reset successfully.");
        }


        public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var hashedToken = _tokenService.HashToken(dto.Token);
            var refreshRecord = await _refreshRepo.GetByHashAsync(hashedToken);

            if (refreshRecord == null || refreshRecord.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token used.");
                throw new UnauthorizedException("Invalid or expired refresh token.");
            }

            var user = await _userManager.FindByIdAsync(refreshRecord.UserId);
            if (user == null)
                throw new UnauthorizedException("User not found.");

            _context.RefreshTokens.Remove(refreshRecord);
            await _context.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles.ToList());

            var newRefreshPlain = _tokenService.GenerateRefreshToken();
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(newRefreshPlain),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            await _refreshRepo.AddAsync(newRefreshToken);
            await _refreshRepo.SaveAsync();

            _logger.LogInformation("Refresh token rotated successfully. UserId: {UserId}", user.Id);

            return ApiResponse<object>.SuccessResponse(new
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshPlain
            }, "Token refreshed successfully.");
        }

        public async Task SendOtpAsync(ApplicationUser user)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new EmailOtp
            {
                UserId = user.Id,
                Email = user.Email!,
                Code = otpCode,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5)
            };

            _context.EmailOtps.Add(otp);
            await _context.SaveChangesAsync();

            await _emailService.SendOtpAsync(user.Email!, otpCode);
            _logger.LogInformation("OTP generated and sent. UserId: {UserId}", user.Id);
        }
    }

}