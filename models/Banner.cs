using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Banner : Entity
    {
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Link { get; set; }

        [Range(0, 1000)]
        public int DisplayOrder { get; set; } = 0;
    }
}
