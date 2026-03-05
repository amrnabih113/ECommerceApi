namespace ECommerce.DTOs.Banners
{
    public class BannerDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Link { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
