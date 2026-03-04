using ECommerce.core.utils;
using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IOrdersRepository : IBaseRepository<Order>
    {
        Task<(IEnumerable<Order> Items, int TotalItems)> GetUserOrdersAsync(string userId, int page, int pageSize);
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<Order?> GetUserOrderWithDetailsAsync(int orderId, string userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
