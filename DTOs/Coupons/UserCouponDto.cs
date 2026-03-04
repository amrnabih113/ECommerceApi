namespace ECommerce.DTOs.Coupons
{
    public class BulkAssignCouponDto
    {
        public List<string> UserIds { get; set; } = new();
    }

    public class UserCouponInfoDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public bool CanUse { get; set; }
        public int UserUsageCount { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
