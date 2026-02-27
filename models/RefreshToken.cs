using ECommerce.Models;

namespace ECommerce.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public required string TokenHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

}