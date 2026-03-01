using ECommerce.Models;

namespace ECommerce.DTOs.Products
{
    public class ProductDetailsDto
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
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}