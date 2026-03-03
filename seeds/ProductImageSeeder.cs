using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class ProductImageSeeder
    {
        public static async Task SeedProductImagesAsync(AppDbContext context)
        {
            if (await context.ProductImages.AnyAsync())
            {
                return;
            }

            var products = await context.Products.ToListAsync();
            var productImages = new List<ProductImage>();

            // Simplified approach: Add 1 main + 1 additional image per product to get 70+ total
            var imageUrls = new[]
            {
                "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500",
                "https://images.unsplash.com/photo-1503341455253-b2e723bb50d5?w=500",
                "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500",
                "https://images.unsplash.com/photo-1595777707802-04b3cfd9b2c4?w=500",
                "https://images.unsplash.com/photo-1553743760-d2bebdc1440a?w=500",
                "https://images.unsplash.com/photo-1556821552-5d63b397dfd1?w=500",
                "https://images.unsplash.com/photo-1542272604-787c62228ecb?w=500",
                "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=500",
                "https://images.unsplash.com/photo-1514432324607-2e467f4af445?w=500",
                "https://images.unsplash.com/photo-1460353581641-37baddab0fa2?w=500"
            };

            int imageIndex = 0;
            foreach (var product in products)
            {
                // Add main image
                productImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = imageUrls[imageIndex % imageUrls.Length],
                    IsMain = true
                });
                imageIndex++;

                // Add 1 additional image per product to ensure we get 70+ total (35 products  2 = 70)
                productImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = imageUrls[imageIndex % imageUrls.Length],
                    IsMain = false
                });
                imageIndex++;
            }

            await context.ProductImages.AddRangeAsync(productImages);
            await context.SaveChangesAsync();
        }
    }
}
