using ECommerce.DTOs.Auth;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(
           IAuthService authService
        )
        {
            _authService = authService;

        }

        // ================= Register =================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        // ================= Verify OTP =================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.VerifyOtpAsync(dto);

            return Ok(response);
        }

        // ================= Login =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        // ================= Refresh Token =================
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RefreshTokenAsync(dto);
            return Ok(response);
        }

        // ================= Forgot Password =================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.ForgotPasswordAsync(dto);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _authService.ResetPasswordAsync(dto);
            return Ok(response);
        }
    }
}
