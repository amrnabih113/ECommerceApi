using ECommerce.Models;

namespace ECommerce.Interfaces.Services
{
    public interface IStripeService
    {
        Task<(string ClientSecret, string PaymentIntentId)> CreatePaymentIntentAsync(Order order);
        Task<Payment?> ConfirmPaymentAsync(string paymentIntentId);
        Task<Payment?> HandlePaymentSuccessAsync(string paymentIntentId);
        Task<Payment?> HandlePaymentFailedAsync(string paymentIntentId, string? failureReason);
        Task<bool> RefundPaymentAsync(string paymentIntentId, decimal? amount = null);
    }
}
