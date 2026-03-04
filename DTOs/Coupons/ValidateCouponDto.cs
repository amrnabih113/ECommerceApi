namespace ECommerce.DTOs.Coupons
{
    public class ValidateCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }

    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal DiscountAmount { get; set; }
        public CouponDto? Coupon { get; set; }
    }
}
