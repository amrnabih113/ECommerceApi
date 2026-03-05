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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate address
                var address = await _context.Set<Address>().FindAsync(dto.AddressId);
                if (address == null)
                {
                    _logger.LogWarning("Order creation blocked: address not found. UserId: {UserId}, AddressId: {AddressId}", userId, dto.AddressId);
                    return ApiResponse<OrderDto>.Error("Invalid shipping address - address not found.");
                }

                if (address.UserId != userId)
                {
                    _logger.LogWarning("Order creation blocked: address ownership mismatch. UserId: {UserId}, AddressId: {AddressId}", userId, dto.AddressId);
                    return ApiResponse<OrderDto>.Error("Invalid shipping address - address does not belong to user.");
                }

                // Calculate order totals
                decimal subTotal = 0;
                var orderItems = new List<OrderItem>();
                if (dto.OrderItems == null || !dto.OrderItems.Any())
                {
                    _logger.LogWarning("Order creation blocked: empty order items. UserId: {UserId}", userId);
                    return ApiResponse<OrderDto>.Error("Order must contain at least one item.");
                }

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
                            _logger.LogWarning("Order creation blocked: variant not found. UserId: {UserId}, ProductVariantId: {ProductVariantId}", userId, item.ProductVariantId.Value);
                            return ApiResponse<OrderDto>.Error($"Product variant {item.ProductVariantId.Value} not found.");
                        }

                        if (variant.ProductId != item.ProductId)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Order creation blocked: variant-product mismatch. UserId: {UserId}, ProductId: {ProductId}, ProductVariantId: {ProductVariantId}", userId, item.ProductId, item.ProductVariantId.Value);
                            return ApiResponse<OrderDto>.Error($"Product variant {item.ProductVariantId.Value} does not belong to product {item.ProductId}.");
                        }

                        // ATOMIC STOCK CHECK: Check stock availability within the locked transaction
                        if (variant.StockQuantity < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Order creation blocked: insufficient variant stock. UserId: {UserId}, ProductVariantId: {ProductVariantId}, Available: {Available}, Requested: {Requested}", userId, item.ProductVariantId.Value, variant.StockQuantity, item.Quantity);
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
                            _logger.LogWarning("Order creation blocked: product missing/inactive. UserId: {UserId}, ProductId: {ProductId}", userId, item.ProductId);
                            return ApiResponse<OrderDto>.Error($"Product {item.ProductId} not found or inactive.");
                        }

                        if (product.HasVariants)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Order creation blocked: variant required. UserId: {UserId}, ProductId: {ProductId}", userId, item.ProductId);
                            return ApiResponse<OrderDto>.Error($"Product {item.ProductId} requires variant selection.");
                        }

                        // ATOMIC STOCK CHECK: Verify stock within locked transaction
                        if (product.StockQuantity < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Order creation blocked: insufficient product stock. UserId: {UserId}, ProductId: {ProductId}, Available: {Available}, Requested: {Requested}", userId, item.ProductId, product.StockQuantity, item.Quantity);
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
                        _logger.LogWarning("Order creation blocked: coupon not found. UserId: {UserId}, CouponCode: {CouponCode}", userId, dto.CouponCode);
                        return ApiResponse<OrderDto>.Error("Coupon code not found.");
                    }

                    // Check if user has access to this coupon
                    var userHasAccess = await _unitOfWork.UserCoupons.UserHasAccessAsync(userId, coupon.Id);
                    if (!userHasAccess)
                    {
                        transaction.Rollback();
                        _logger.LogWarning("Order creation blocked: coupon access denied. UserId: {UserId}, CouponId: {CouponId}", userId, coupon.Id);
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

                var createdOrder = await _unitOfWork.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Order created successfully. OrderId: {OrderId}, UserId: {UserId}, TotalAmount: {TotalAmount}", createdOrder.Id, userId, totalAmount);

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
            var order = await _unitOfWork.Orders.GetUserOrderWithDetailsAsync(orderId, userId);
            if (order == null)
            {
                _logger.LogWarning("Order cancellation blocked: order not found/owned. UserId: {UserId}, OrderId: {OrderId}", userId, orderId);
                return ApiResponse.ErrorResponse("Order not found.");
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid)
            {
                _logger.LogWarning("Order cancellation blocked: invalid status. UserId: {UserId}, OrderId: {OrderId}, CurrentStatus: {Status}", userId, orderId, order.Status);
                return ApiResponse.ErrorResponse("Only pending or paid orders can be cancelled.");
            }

            // Check if order was paid and has payment - refund BEFORE changing status
            if (order.Status == OrderStatus.Paid && order.Payment != null)
            {
                var refundSuccess = await _stripeService.RefundPaymentAsync(order.Payment.StripePaymentIntentId);

                if (refundSuccess)
                {
                }
                else
                {
                    _logger.LogError("Refund failed while cancelling order {OrderId}", orderId);
                    return ApiResponse.ErrorResponse("Failed to process refund. Order cancellation aborted.");
                }
            }

            // Now cancel the order
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order cancelled. OrderId: {OrderId}, UserId: {UserId}", orderId, userId);
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

            _logger.LogInformation("Payment intent created. OrderId: {OrderId}, UserId: {UserId}, PaymentIntentId: {PaymentIntentId}", orderId, userId, paymentIntentId);

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
