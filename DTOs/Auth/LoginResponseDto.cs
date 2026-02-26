namespace ECommerce.DTOs.Auth
{
    public class LoginResponseDto
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? ImageUrl { get; set; }
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }

    }
}