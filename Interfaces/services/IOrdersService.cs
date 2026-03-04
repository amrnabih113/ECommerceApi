using ECommerce.DTOs;
using ECommerce.DTOs.Orders;
using ECommerce.DTOs.Payments;

namespace ECommerce.Interfaces.Services
{
    public interface IOrdersService
    {
        Task<ApiResponse<PageResult<OrderDto>>> GetAllOrdersAsync(int page, int pageSize);
        Task<ApiResponse<PageResult<OrderDto>>> GetUserOrdersAsync(string userId, int page, int pageSize);
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<OrderDto>> GetUserOrderByIdAsync(int orderId, string userId);
        Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto, string userId);
        Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<ApiResponse> CancelOrderAsync(int orderId, string userId);
        Task<ApiResponse<PaymentIntentResponseDto>> CreatePaymentIntentAsync(int orderId, string userId);
    }
}
