namespace ECommerce.DTOs.Brands
{
    public class BrandUpdateDto
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public required string LogoUrl { get; set; }
    }
}