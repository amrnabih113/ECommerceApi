using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Otp { get; set; }

        [Required]
        [MinLength(6)]
        public required string NewPassword { get; set; }
    }
}