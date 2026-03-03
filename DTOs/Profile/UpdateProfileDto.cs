using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Profile
{
    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
