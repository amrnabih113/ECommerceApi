
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Coupon : Entity
    {
        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } // Fixed amount

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int UsageLimit { get; set; }

        public int UsedCount { get; set; }
    }
}