namespace ECommerce.DTOs.Products
{
    public class ProductQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasVariants { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
    }
}