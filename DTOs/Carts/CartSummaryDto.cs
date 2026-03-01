namespace ECommerce.DTOs.Carts
{
    public class CartSummaryDto
    {
        public int CartId { get; set; }
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
