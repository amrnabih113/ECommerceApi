using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.interfaces;
using ECommerce.models;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshRepo,
            AppDbContext context,
            IEmailService emailService
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshRepo = refreshRepo;
            _context = context;
            _emailService = emailService;
        }

        // ================= Register =================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { Email = dto.Email, UserName = dto.Email };

            var userResult = await _userManager.CreateAsync(user, dto.Password);
            if (!userResult.Succeeded)
                return BadRequest(userResult.Errors);

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

            var createduser = await _userManager.FindByEmailAsync(user.Email);
            if (createduser == null)
                return NotFound("User not found after creation.");
            var responseDto = new RegisterResponseDto { UserId = createduser.Id, Email = createduser.Email! };

            return Ok(new ApiResponse<RegisterResponseDto>
            {
                Message = "Registration successful. Please verify your email.",
                Data = responseDto
            });
        }

        // ================= Verify OTP =================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get OTP record by email
            var otpRecord = await _context
                .EmailOtps.Include(o => o.User)
                .FirstOrDefaultAsync(o => o.User.Email == dto.Email && o.Code == dto.Otp);

            if (otpRecord == null)
                return BadRequest("Invalid OTP.");

            if (otpRecord.ExpiryDate < DateTime.UtcNow)
                return BadRequest("OTP expired.");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return NotFound("User not found.");

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Remove OTP
            _context.EmailOtps.Remove(otpRecord);
            await _context.SaveChangesAsync();

            return Ok("Email verified successfully.");
        }

        // ================= Login =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            if (!user.EmailConfirmed)
            {
                // TODO: Send OTP again.
                return Unauthorized("Email is not verified.");

            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return Unauthorized("Invalid credentials.");

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

            return Ok(new { Token = accessToken, RefreshToken = refreshTokenPlain });
        }

        // ================= Refresh Token =================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            // Hash incoming token
            var hashedToken = _tokenService.HashToken(dto.Token);

            // Get refresh token from repo
            var refreshTokenRecord = await _refreshRepo.GetByHashAsync(hashedToken);
            if (refreshTokenRecord == null || refreshTokenRecord.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            // Get user
            var user = await _userManager.FindByIdAsync(refreshTokenRecord.UserId);
            if (user == null)
                return Unauthorized("User not found.");

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

            return Ok(new { Token = newAccessToken, RefreshToken = newRefreshTokenPlain });
        }

        // ================= Forgot Password =================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Ok(); // don't reveal existence

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
            await _emailService.SendOtpAsync(user.Email, otpCode);

            return Ok("Password reset email sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Invalid email");

            var otpRecord = await _context.EmailOtps.FirstOrDefaultAsync(o =>
                o.Email == dto.Email && o.Code == dto.Otp
            );

            if (otpRecord == null)
                return BadRequest("Invalid OTP");

            if (otpRecord.ExpiryDate < DateTime.UtcNow)
                return BadRequest("OTP expired");

            var removePassword = await _userManager.RemovePasswordAsync(user);
            if (!removePassword.Succeeded)
                return BadRequest(removePassword.Errors);

            var addPassword = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            if (!addPassword.Succeeded)
                return BadRequest(addPassword.Errors);

            _context.EmailOtps.Remove(otpRecord);
            await _context.SaveChangesAsync();

            return Ok("Password reset successful");
        }
    }
}
