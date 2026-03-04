using System.ComponentModel.DataAnnotations;
using ECommerce.core.utils;

namespace ECommerce.DTOs.Coupons
{
    public class CreateCouponDto
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MinimumOrderAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaximumDiscountAmount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        [Range(1, int.MaxValue)]
        public int? UsageLimit { get; set; }
    }
}
