using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Products
{
    public class ProductImageUpdateDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;

        public bool IsMain { get; set; }
    }
}