using System.ComponentModel.DataAnnotations;
using ECommerce.core.utils;

namespace ECommerce.DTOs.Coupons
{
    public class UpdateCouponDto
    {
        [MaxLength(200)]
        public string? Description { get; set; }

        public DiscountType? DiscountType { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinimumOrderAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaximumDiscountAmount { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidUntil { get; set; }

        public bool? IsActive { get; set; }

        [Range(1, int.MaxValue)]
        public int? UsageLimit { get; set; }
    }
}
