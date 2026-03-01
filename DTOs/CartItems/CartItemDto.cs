namespace ECommerce.DTOs.CartItems
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? VariantSize { get; set; }
        public string? VariantColor { get; set; }
        public decimal? AdditionalPrice { get; set; }
        public string? VariantImageUrl { get; set; }
        public int CartId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

