using ECommerce.DTOs;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ECommerce.Controllers
{
    /// <summary>
    /// Stripe webhook handler
    /// </summary>
    [ApiController]
    [Route("api/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WebhooksController> _logger;

        public WebhooksController(
            IStripeService stripeService,
            IConfiguration configuration,
            ILogger<WebhooksController> logger)
        {
            _stripeService = stripeService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Stripe webhook endpoint - handles payment events
        /// Receives events from Stripe when payment status changes
        /// </summary>
        [HttpPost("stripe")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = _configuration["Stripe:WebhookSecret"];

            try
            {
                _logger.LogInformation("Stripe webhook received.");

                // TODO: For production, use ConstructEvent for signature verification
                // For local testing with Stripe CLI, we skip signature verification
                var stripeEvent = EventUtility.ParseEvent(json);

                if (stripeEvent == null)
                {
                    _logger.LogError("Failed to parse Stripe event from JSON");
                    return BadRequest(ApiResponse.ErrorResponse("Invalid webhook event"));
                }

                _logger.LogInformation("Stripe webhook event parsed. EventType: {EventType}", stripeEvent.Type);

                // Handle the event
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data?.Object as PaymentIntent;

                        if (paymentIntent == null)
                        {
                            _logger.LogError("PaymentIntent object is null in event data");
                            return BadRequest(ApiResponse.ErrorResponse("Invalid payment intent in webhook event"));
                        }

                        var result = await _stripeService.HandlePaymentSuccessAsync(paymentIntent.Id);

                        if (result == null)
                        {
                            _logger.LogWarning("Payment record not found for successful webhook PaymentIntentId: {PaymentIntentId}", paymentIntent.Id);
                            return BadRequest(ApiResponse.ErrorResponse($"Payment not found for PaymentIntent: {paymentIntent.Id}. Make sure to create the payment intent through POST /api/orders/{{orderId}}/payment first."));
                        }
                        _logger.LogInformation("Payment success webhook processed. PaymentIntentId: {PaymentIntentId}", paymentIntent.Id);
                        break;

                    case "payment_intent.payment_failed":
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (failedIntent != null)
                        {
                            await _stripeService.HandlePaymentFailedAsync(
                                failedIntent.Id,
                                failedIntent.LastPaymentError?.Message ?? "Payment failed"
                            );
                            _logger.LogWarning("Payment failed webhook received. PaymentIntentId: {PaymentIntentId}", failedIntent.Id);
                        }
                        break;

                    case "charge.refunded":
                        var charge = stripeEvent.Data.Object as Charge;
                        if (charge != null && charge.PaymentIntentId != null)
                        {
                            _logger.LogInformation("Charge refunded webhook received. PaymentIntentId: {PaymentIntentId}", charge.PaymentIntentId);
                            // Refund is already handled in RefundPaymentAsync method
                        }
                        break;

                    default:
                        _logger.LogInformation("Unhandled Stripe webhook event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe webhook error");
                return BadRequest(ApiResponse.ErrorResponse($"Webhook error: {e.Message}"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Webhook processing error");
                return StatusCode(500, ApiResponse.ErrorResponse($"Internal error: {e.Message}"));
            }
        }
    }
}
