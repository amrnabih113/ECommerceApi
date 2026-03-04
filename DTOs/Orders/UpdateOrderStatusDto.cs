namespace ECommerce.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
    }
}
