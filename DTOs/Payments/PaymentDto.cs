namespace ECommerce.DTOs.Payments
{
    public class CreatePaymentIntentDto
    {
        public int OrderId { get; set; }
    }

    public class PaymentIntentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class ConfirmPaymentDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
