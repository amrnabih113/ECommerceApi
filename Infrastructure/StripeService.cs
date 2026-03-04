using ECommerce.core.utils;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;
using Microsoft.Extensions.Configuration;
using Stripe;
using PaymentMethod = ECommerce.core.utils.PaymentMethod;

namespace ECommerce.Infrastructure
{
    public class StripeService : IStripeService
    {
        private readonly IPaymentsRepository _paymentsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IConfiguration _configuration;

        public StripeService(
            IPaymentsRepository paymentsRepository,
            IOrdersRepository ordersRepository,
            IConfiguration configuration)
        {
            _paymentsRepository = paymentsRepository;
            _ordersRepository = ordersRepository;
            _configuration = configuration;

            // Set Stripe API key
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<(string ClientSecret, string PaymentIntentId)> CreatePaymentIntentAsync(Models.Order order)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(order.TotalAmount * 100), // Convert to cents
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.Id.ToString() },
                    { "user_id", order.UserId ?? string.Empty }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Create Payment record
            var payment = new Payment
            {
                OrderId = order.Id,
                StripePaymentIntentId = paymentIntent.Id,
                Amount = order.TotalAmount,
                Currency = "USD",
                Status = PaymentStatus.Pending,
                PaymentMethod = PaymentMethod.Card
            };

            await _paymentsRepository.AddAsync(payment);

            return (paymentIntent.ClientSecret, paymentIntent.Id);
        }

        public async Task<Payment?> ConfirmPaymentAsync(string paymentIntentId)
        {
            var payment = await _paymentsRepository.GetByPaymentIntentIdAsync(paymentIntentId);
            if (payment == null)
                return null;

            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            if (paymentIntent.Status == "succeeded")
            {
                payment.Status = PaymentStatus.Succeeded;
                payment.StripeChargeId = paymentIntent.LatestChargeId;
                payment.PaidAt = DateTime.UtcNow;

                // Update order status
                await _ordersRepository.UpdateOrderStatusAsync(payment.OrderId, core.utils.OrderStatus.Paid);
            }
            else if (paymentIntent.Status == "requires_payment_method" || paymentIntent.Status == "canceled")
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = paymentIntent.CancellationReason ?? "Payment failed";
            }
            else
            {
                payment.Status = PaymentStatus.Processing;
            }

            payment.UpdatedAt = DateTime.UtcNow;
            await _paymentsRepository.UpdateAsync(payment);

            return payment;
        }

        public async Task<Payment?> HandlePaymentSuccessAsync(string paymentIntentId)
        {
            try
            {
                Console.WriteLine($"[HandlePaymentSuccessAsync] Looking up payment for PaymentIntent: {paymentIntentId}");

                var payment = await _paymentsRepository.GetByPaymentIntentIdAsync(paymentIntentId);
                if (payment == null)
                {
                    Console.WriteLine($"[WARNING] Payment not found for PaymentIntent: {paymentIntentId}");
                    return null;
                }

                Console.WriteLine($"[HandlePaymentSuccessAsync] Found payment ID: {payment.Id}, Order ID: {payment.OrderId}");

                payment.Status = PaymentStatus.Succeeded;
                payment.PaidAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                // Get charge ID from Stripe
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);
                payment.StripeChargeId = paymentIntent.LatestChargeId;

                Console.WriteLine($"[HandlePaymentSuccessAsync] Updating payment {payment.Id}");
                await _paymentsRepository.UpdateAsync(payment);

                // Update order status to Paid
                Console.WriteLine($"[HandlePaymentSuccessAsync] Updating order {payment.OrderId} to Paid");
                await _ordersRepository.UpdateOrderStatusAsync(payment.OrderId, core.utils.OrderStatus.Paid);

                Console.WriteLine($"[SUCCESS] Payment {payment.Id} updated to Succeeded for Order {payment.OrderId}");

                return payment;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] HandlePaymentSuccessAsync failed: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Payment?> HandlePaymentFailedAsync(string paymentIntentId, string? failureReason)
        {
            var payment = await _paymentsRepository.GetByPaymentIntentIdAsync(paymentIntentId);
            if (payment == null)
                return null;

            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = failureReason ?? "Payment failed";
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentsRepository.UpdateAsync(payment);

            // Update order status to Cancelled
            await _ordersRepository.UpdateOrderStatusAsync(payment.OrderId, core.utils.OrderStatus.Cancelled);

            return payment;
        }

        public async Task<bool> RefundPaymentAsync(string paymentIntentId, decimal? amount = null)
        {
            var payment = await _paymentsRepository.GetByPaymentIntentIdAsync(paymentIntentId);
            if (payment == null || payment.Status != PaymentStatus.Succeeded)
                return false;

            try
            {
                var refundService = new RefundService();
                var refundOptions = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId
                };

                if (amount.HasValue)
                {
                    refundOptions.Amount = (long)(amount.Value * 100); // Convert to cents
                }

                var refund = await refundService.CreateAsync(refundOptions);

                if (refund.Status == "succeeded")
                {
                    var refundedAmount = refund.Amount / 100m;

                    if (refundedAmount >= payment.Amount)
                    {
                        payment.Status = PaymentStatus.Refunded;
                        payment.RefundedAmount = payment.Amount;
                    }
                    else
                    {
                        payment.Status = PaymentStatus.PartiallyRefunded;
                        payment.RefundedAmount = (payment.RefundedAmount ?? 0) + refundedAmount;
                    }

                    payment.RefundedAt = DateTime.UtcNow;
                    payment.UpdatedAt = DateTime.UtcNow;
                    await _paymentsRepository.UpdateAsync(payment);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
