using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        public required string Token { get; set; }
    }
}