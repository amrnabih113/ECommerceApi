namespace ECommerce.DTOs.Products
{
    public class ProductDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = default!;
    }
}