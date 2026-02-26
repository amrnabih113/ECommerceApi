using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}