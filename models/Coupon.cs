
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.core.utils;

namespace ECommerce.Models
{
    public class Coupon : Entity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = default!;

        [MaxLength(200)]
        public string? Description { get; set; }

        // Discount Type: Percentage or Fixed Amount
        [Column(TypeName = "nvarchar(24)")]
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

        // Percentage (0-100) OR Fixed Amount based on DiscountType
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        // Minimum order amount to apply coupon
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumOrderAmount { get; set; }

        // Maximum discount amount (for percentage discounts)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaximumDiscountAmount { get; set; }

        public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
        public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; } = true;

        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;

        public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}