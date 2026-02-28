namespace ECommerce.DTOs.Categories
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string ImageUrl { get; set; }
        public int ProductsCount { get; set; }
        public bool IsActive { get; set; }
        public int? ParentCategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}