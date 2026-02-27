using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.Models;

namespace ECommerce.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string TokenHash { get; set; }

        [Required]

        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public required string UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

}