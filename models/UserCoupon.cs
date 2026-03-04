using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    
    public class UserCoupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int CouponId { get; set; }
        public Coupon Coupon { get; set; } = null!;

        public bool CanUse { get; set; } = true;

        public int UserUsageCount { get; set; } = 0;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
