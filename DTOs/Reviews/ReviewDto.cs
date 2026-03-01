namespace ECommerce.DTOs.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = string.Empty;
        public string? UserImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}