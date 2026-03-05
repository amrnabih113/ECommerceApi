using AutoMapper;
using ECommerce.core.utils;
using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.DTOs.Orders;
using ECommerce.DTOs.Payments;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripeService _stripeService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersService> _logger;

        public OrdersService(
            IUnitOfWork unitOfWork,
            IStripeService stripeService,
            AppDbContext context,
            IMapper mapper,
            ILogger<OrdersService> logger)
        {
            _unitOfWork = unitOfWork;
            _stripeService = stripeService;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<PageResult<OrderDto>>> GetAllOrdersAsync(int page, int pageSize)
        {
            var (items, totalItems) = await _unitOfWork.Orders.GetPagedAsync(page, pageSize);
            var orderDtos = _mapper.Map<List<OrderDto>>(items);

            var pageResult = new PageResult<OrderDto>
            {
                Items = orderDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return ApiResponse<PageResult<OrderDto>>.SuccessResponse(pageResult, "Orders retrieved successfully.");
        }

        public async Task<ApiResponse<PageResult<OrderDto>>> GetUserOrdersAsync(string userId, int page, int pageSize)
        {
            var (items, totalItems) = await _unitOfWork.Orders.GetUserOrdersAsync(userId, page, pageSize);
            var orderDtos = _mapper.Map<List<OrderDto>>(items);

            var pageResult = new PageResult<OrderDto>
            {
                Items = orderDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return ApiResponse<PageResult<OrderDto>>.SuccessResponse(pageResult, "Orders retrieved successfully.");
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return ApiResponse<OrderDto>.Error("Order not found.");

            var orderDto = _mapper.Map<OrderDto>(order);
            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order retrieved successfully.");
        }

        public async Task<ApiResponse<OrderDto>> GetUserOrderByIdAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.Orders.GetUserOrderWithDetailsAsync(orderId, userId);
            if (order == null)
                return ApiResponse<OrderDto>.Error("Order not found.");

            var orderDto = _mapper.Map<OrderDto>(order);
            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order retrieved successfully.");
        }

        public async Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto, string userId)
        {
            _logger.LogInformation("CreateOrderAsync called for user {UserId} with {ItemCount} items", userId, dto.OrderItems?.Count ?? 0);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Validating address {AddressId} for user {UserId}", dto.AddressId, userId);

                // Validate address
                var address = await _context.Set<Address>().FindAsync(dto.AddressId);
                if (address == null)
                {
                    _logger.LogWarning("Address {AddressId} not found", dto.AddressId);
                    return ApiResponse<OrderDto>.Error("Invalid shipping address - address not found.");
                }

                if (address.UserId != userId)
                {
                    _logger.LogWarning("Address {AddressId} does not belong to user {UserId}", dto.AddressId, userId);
                    return ApiResponse<OrderDto>.Error("Invalid shipping address - address does not belong to user.");
                }

                _logger.LogInformation("Address validated successfully");

                // Calculate order totals
                decimal subTotal = 0;
                var orderItems = new List<OrderItem>();
                if (dto.OrderItems == null || !dto.OrderItems.Any())
                {
                    _logger.LogWarning("Order has no items");
                    return ApiResponse<OrderDto>.Error("Order must contain at least one item.");
                }
                _logger.LogInformation("Processing {Count} order items", dto.OrderItems.Count);

                foreach (var item in dto.OrderItems)
                {
                    Product? product;
                    decimal unitPrice;

                    if (item.ProductVariantId.HasValue)
                    {
                        // RACE CONDITION PROTECTION: Get variant with row-level locking
                        // This prevents two users from buying the last item simultaneously
                        var variant = await _unitOfWork.ProductVariants.GetByIdWithLockAsync(item.ProductVariantId.Value);

                        if (variant == null)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Product variant {item.ProductVariantId.Value} not found.");
                        }

                        if (variant.ProductId != item.ProductId)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Product variant {item.ProductVariantId.Value} does not belong to product {item.ProductId}.");
                        }

                        // ATOMIC STOCK CHECK: Check stock availability within the locked transaction
                        if (variant.StockQuantity < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Insufficient stock for variant. Available: {variant.StockQuantity}, Requested: {item.Quantity}");
                        }

                        // PREVENT NEGATIVE STOCK: Reduce stock only after validation
                        variant.StockQuantity -= item.Quantity;

                        // Load product info for the order item
                        product = await _context.Set<Product>().FindAsync(item.ProductId);
                        if (product == null)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Product {item.ProductId} not found.");
                        }

                        unitPrice = product.Price + (variant.AdditionalPrice ?? 0);
                    }
                    else
                    {
                        // RACE CONDITION PROTECTION: Get product with row-level locking
                        product = await _unitOfWork.Products.GetByIdWithLockAsync(item.ProductId);

                        if (product == null || !product.IsActive)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Product {item.ProductId} not found or inactive.");
                        }

                        if (product.HasVariants)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Product {item.ProductId} requires variant selection.");
                        }

                        // ATOMIC STOCK CHECK: Verify stock within locked transaction
                        if (product.StockQuantity < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return ApiResponse<OrderDto>.Error($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                        }

                        // PREVENT NEGATIVE STOCK: Reduce stock only after validation
                        product.StockQuantity -= item.Quantity;

                        unitPrice = product.Price;
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice
                    };

                    orderItems.Add(orderItem);
                    subTotal += unitPrice * item.Quantity;
                }

                // Apply coupon if provided
                decimal couponDiscount = 0;
                int? couponId = null;

                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    var coupon = await _unitOfWork.Coupons.GetByCodeAsync(dto.CouponCode);

                    if (coupon == null)
                    {
                        transaction.Rollback();
                        return ApiResponse<OrderDto>.Error("Coupon code not found.");
                    }

                    // Check if user has access to this coupon
                    var userHasAccess = await _unitOfWork.UserCoupons.UserHasAccessAsync(userId, coupon.Id);
                    if (!userHasAccess)
                    {
                        transaction.Rollback();
                        return ApiResponse<OrderDto>.Error("You don't have access to this coupon.");
                    }

                    if (coupon.IsActive &&
                        DateTime.UtcNow >= coupon.ValidFrom && DateTime.UtcNow <= coupon.ValidUntil)
                    {
                        if (!coupon.UsageLimit.HasValue || coupon.UsedCount < coupon.UsageLimit.Value)
                        {
                            if (!coupon.MinimumOrderAmount.HasValue || subTotal >= coupon.MinimumOrderAmount.Value)
                            {
                                // Calculate discount
                                if (coupon.DiscountType == DiscountType.Percentage)
                                {
                                    couponDiscount = subTotal * (coupon.DiscountValue / 100m);

                                    if (coupon.MaximumDiscountAmount.HasValue && couponDiscount > coupon.MaximumDiscountAmount.Value)
                                        couponDiscount = coupon.MaximumDiscountAmount.Value;
                                }
                                else
                                {
                                    couponDiscount = coupon.DiscountValue;
                                }

                                if (couponDiscount > subTotal)
                                    couponDiscount = subTotal;

                                couponId = coupon.Id;
                                await _unitOfWork.Coupons.IncrementUsedCountAsync(coupon.Id);
                                await _unitOfWork.UserCoupons.IncrementUserUsageCountAsync(userId, coupon.Id);
                            }
                        }
                    }
                }

                // Calculate totals (you can add tax and shipping logic here)
                decimal shippingPrice = 10m; // Fixed shipping for now
                decimal taxAmount = (subTotal - couponDiscount) * 0.1m; // 10% tax
                decimal totalAmount = subTotal + shippingPrice + taxAmount - couponDiscount;

                // Create order
                _logger.LogInformation("Creating order with SubTotal: {SubTotal}, TotalAmount: {TotalAmount}", subTotal, totalAmount);

                var order = new Order
                {
                    UserId = userId,
                    SubTotal = subTotal,
                    ShippingPrice = shippingPrice,
                    TaxAmount = taxAmount,
                    CouponDiscount = couponDiscount,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    ShippingAddressId = dto.AddressId,
                    CouponId = couponId,
                    Notes = dto.Notes,
                    OrderItems = orderItems
                };

                _logger.LogInformation("Saving order to database");
                var createdOrder = await _unitOfWork.Orders.AddAsync(order);
                _logger.LogInformation("Order saved with ID: {OrderId}", createdOrder.Id);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Load full order details
                var fullOrder = await _unitOfWork.Orders.GetOrderWithDetailsAsync(createdOrder.Id);
                var orderDto = _mapper.Map<OrderDto>(fullOrder);

                return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}: {ErrorMessage}", userId, ex.Message);
                await transaction.RollbackAsync();
                return ApiResponse<OrderDto>.Error($"Failed to create order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return ApiResponse<OrderDto>.Error("Order not found.");

            // Parse status string to enum
            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
                return ApiResponse<OrderDto>.Error($"Invalid status: {dto.Status}. Valid values: Pending, Paid, Shipped, Delivered, Cancelled");

            // Business rules for status transitions
            if (order.Status == OrderStatus.Cancelled)
                return ApiResponse<OrderDto>.Error("Cannot update a cancelled order.");

            if (order.Status == OrderStatus.Delivered)
                return ApiResponse<OrderDto>.Error("Cannot update a delivered order.");

            order.Status = newStatus;
            if (!string.IsNullOrWhiteSpace(dto.Notes))
                order.Notes = dto.Notes;

            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            var orderDto = _mapper.Map<OrderDto>(order);
            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order status updated successfully.");
        }

        public async Task<ApiResponse> CancelOrderAsync(int orderId, string userId)
        {
            _logger.LogInformation("CancelOrderAsync called for order {OrderId} by user {UserId}", orderId, userId);

            var order = await _unitOfWork.Orders.GetUserOrderWithDetailsAsync(orderId, userId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found for user {UserId}", orderId, userId);
                return ApiResponse.ErrorResponse("Order not found.");
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid)
            {
                _logger.LogWarning("Cannot cancel order {OrderId} with status {Status}", orderId, order.Status);
                return ApiResponse.ErrorResponse("Only pending or paid orders can be cancelled.");
            }

            // Check if order was paid and has payment - refund BEFORE changing status
            if (order.Status == OrderStatus.Paid && order.Payment != null)
            {
                _logger.LogInformation("Initiating refund for paid order {OrderId}", orderId);
                var refundSuccess = await _stripeService.RefundPaymentAsync(order.Payment.StripePaymentIntentId);

                if (refundSuccess)
                {
                    _logger.LogInformation("Refund initiated for payment {PaymentId}", order.Payment.Id);
                }
                else
                {
                    _logger.LogWarning("Refund failed for order {OrderId}", orderId);
                    return ApiResponse.ErrorResponse("Failed to process refund. Order cancellation aborted.");
                }
            }

            // Now cancel the order
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
            return ApiResponse.SuccessResponse("Order cancelled successfully.");
        }

        public async Task<ApiResponse<PaymentIntentResponseDto>> CreatePaymentIntentAsync(int orderId, string userId)
        {
            var order = await _unitOfWork.Orders.GetUserOrderWithDetailsAsync(orderId, userId);
            if (order == null)
                return ApiResponse<PaymentIntentResponseDto>.Error("Order not found.");

            if (order.Status != OrderStatus.Pending)
                return ApiResponse<PaymentIntentResponseDto>.Error("Only pending orders can be paid.");

            // Check if payment already exists
            if (order.Payment != null)
                return ApiResponse<PaymentIntentResponseDto>.Error("Payment already initiated for this order.");

            var (clientSecret, paymentIntentId) = await _stripeService.CreatePaymentIntentAsync(order);

            var response = new PaymentIntentResponseDto
            {
                ClientSecret = clientSecret,
                PaymentIntentId = paymentIntentId,
                Amount = order.TotalAmount,
                Currency = "USD"
            };

            return ApiResponse<PaymentIntentResponseDto>.SuccessResponse(response, "Payment intent created successfully.");
        }
    }
}
