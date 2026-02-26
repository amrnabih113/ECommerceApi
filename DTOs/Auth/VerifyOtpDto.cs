using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Otp { get; set; }
    }
}