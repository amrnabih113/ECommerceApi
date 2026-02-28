using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Brands
{
    public class BrandUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        public string? Description { get; set; }
        [Required]
        [Url]
        public required string LogoUrl { get; set; }
    }
}