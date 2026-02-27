
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Review : Entity
    {

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public int UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}