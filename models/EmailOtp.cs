namespace ECommerce.Models
{
    public class EmailOtp
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string Code { get; set; }
        public DateTime ExpiryDate { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
