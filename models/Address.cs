
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Address : Entity
    {
        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

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