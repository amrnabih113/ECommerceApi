
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Address : Entity
    {
        [Required]
        public string? UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Country { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string City { get; set; } = default!;

        [Required]
        [MaxLength(500)]
        public string Street { get; set; } = default!;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = default!;
    }
}