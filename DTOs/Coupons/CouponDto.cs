using ECommerce.core.utils;

namespace ECommerce.DTOs.Coupons
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
