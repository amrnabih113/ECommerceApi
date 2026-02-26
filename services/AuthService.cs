
using ECommerce.core.Exceptions;
using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.services;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IEmailService _emailService;

        private readonly AppDbContext _context;

        public AuthService(AppDbContext context, UserManager<ApplicationUser> userManager, ITokenService tokenService, IRefreshTokenRepository refreshRepo, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshRepo = refreshRepo;
            _emailService = emailService;

        }

        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser { Email = dto.Email, UserName = dto.Email };

            var userResult = await _userManager.CreateAsync(user, dto.Password);
            if (!userResult.Succeeded)
                throw new BadRequestException(string.Join(", ", userResult.Errors.Select(e => e.Description)));

            await SendOtpAsync(user);

            var createduser = await _userManager.FindByEmailAsync(user.Email);
            if (createduser == null)
                throw new BadRequestException("User not found after creation.");
            var responseDto = new RegisterResponseDto { UserId = createduser.Id, Email = createduser.Email! };

            return new ApiResponse<RegisterResponseDto> { Data = responseDto, Message = "Registration successful. Please verify your email." };
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new ApiResponse { Message = "Otp sent successfully." };

            await SendOtpAsync(user);
            return new ApiResponse { Message = "Otp sent successfully." };

        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid credentials.");

            if (!user.EmailConfirmed)
            {
                await SendOtpAsync(user);
                throw new Exception("Email is not verified.");

            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                throw new Exception("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles.ToList());

            // Generate refresh token using TokenService
            var refreshTokenPlain = _tokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(refreshTokenPlain),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(15),
            };
            await _refreshRepo.AddAsync(refreshToken);
            await _refreshRepo.SaveAsync();

            return new ApiResponse<LoginResponseDto> { Message = "Login successful.", Data = new LoginResponseDto { UserId = user.Id, Email = user.Email, FullName = user.FullName, Address = user.Address, ImageUrl = user.ImageUrl, AccessToken = accessToken, RefreshToken = refreshTokenPlain } };
        }

        public async Task<ApiResponse> VerifyOtpAsync(VerifyOtpDto dto)
        {

            // Get OTP record by email
            var otpRecord = await _context
                .EmailOtps.Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User.Email == dto.Email && o.Code == dto.Otp);

            if (otpRecord == null)
                throw new InvalidOtpException();

            if (otpRecord.ExpiryDate < DateTime.UtcNow)
                throw new OtpExpiredException();

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("User not found.");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Remove OTP
            _context.EmailOtps.Remove(otpRecord);
            await _context.SaveChangesAsync();

            return new ApiResponse { Message = "Email verified successfully.", };
        }

        public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto)
        {
            // Hash incoming token
            var hashedToken = _tokenService.HashToken(dto.Token);

            // Get refresh token from repo
            var refreshTokenRecord = await _refreshRepo.GetByHashAsync(hashedToken);
            if (refreshTokenRecord == null || refreshTokenRecord.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            // Get user
            var user = await _userManager.FindByIdAsync(refreshTokenRecord.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            // Remove old token
            _context.RefreshTokens.Remove(refreshTokenRecord);
            await _context.SaveChangesAsync();

            // Generate new access token
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.GenerateAccessToken(user, roles.ToList());

            // Generate new refresh token
            var newRefreshTokenPlain = _tokenService.GenerateRefreshToken();
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(newRefreshTokenPlain),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };
            await _refreshRepo.AddAsync(newRefreshToken);
            await _refreshRepo.SaveAsync();

            return new ApiResponse<object> { Message = "Token refreshed successfully.", Data = new { AccessToken = newAccessToken, RefreshToken = newRefreshTokenPlain } };
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
                throw new BadRequestException("Invalid OTP");

            if (otpRecord.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("OTP expired");

            var removePassword = await _userManager.RemovePasswordAsync(user);
            if (!removePassword.Succeeded)
                throw new BadRequestException(string.Join(", ", removePassword.Errors.Select(e => e.Description)));

            var addPassword = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addPassword.Succeeded)
                throw new BadRequestException(string.Join(", ", addPassword.Errors.Select(e => e.Description)));

            _context.EmailOtps.Remove(otpRecord);
            await _context.SaveChangesAsync();

            return new ApiResponse { Message = "Password reset successfully." };
        }
        
        public async Task SendOtpAsync(ApplicationUser user)
        {
            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();
            var otp = new EmailOtp
            {
                UserId = user.Id,
                Email = user.Email,
                Code = otpCode,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5),
            };
            _context.EmailOtps.Add(otp);
            await _context.SaveChangesAsync();
            // Send OTP email
            await _emailService.SendOtpAsync(user.Email, otpCode);
        }


    }

}