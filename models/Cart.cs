
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Cart : Entity
    {
        [Required]
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}