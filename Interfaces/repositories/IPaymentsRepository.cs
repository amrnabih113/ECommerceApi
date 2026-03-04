using ECommerce.Models;

namespace ECommerce.Interfaces.Repositories
{
    public interface IPaymentsRepository : IBaseRepository<Payment>
    {
        Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId);
        Task<Payment?> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId);
    }
}
