using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommerce.core.utils;

namespace ECommerce.Models
{
    public class Payment : Entity
    {
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string StripePaymentIntentId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? StripeChargeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "USD";

        [Required]
        [Column(TypeName = "nvarchar(24)")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Column(TypeName = "nvarchar(24)")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? RefundedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundedAmount { get; set; }
    }
}
