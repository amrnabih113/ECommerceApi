using ECommerce.DTOs.Coupons;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal CouponDiscount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public CouponDto? Coupon { get; set; }
        public AddressDto? ShippingAddress { get; set; }
        public PaymentDto? Payment { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public int? ProductVariantId { get; set; }
        public string? VariantSize { get; set; }
        public string? VariantColor { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
    }

    public class AddressDto
    {
        public int Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }

    public class CreateAddressDto
    {
        [Required]
        [MaxLength(200)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;
    }

    public class UpdateAddressDto
    {
        [Required]
        [MaxLength(200)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public string StripePaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }
}
