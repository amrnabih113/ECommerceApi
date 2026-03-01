using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Reviews
{
    public class ReviewUpdateDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = default!;
    }
}