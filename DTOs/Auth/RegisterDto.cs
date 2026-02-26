using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }
    }
}

