using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Banners
{
    public class BannerUpdateDto
    {
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "Link cannot exceed 500 characters")]
        [Url(ErrorMessage = "Link must be a valid URL")]
        public string? Link { get; set; }

        [Range(0, 1000, ErrorMessage = "DisplayOrder must be between 0 and 1000")]
        public int? DisplayOrder { get; set; }
    }
}
