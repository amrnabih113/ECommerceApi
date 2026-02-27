
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Brand : Entity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? LogoUrl { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}