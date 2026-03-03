using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class ProductVariantSeeder
    {
        public static async Task SeedProductVariantsAsync(AppDbContext context)
        {
            if (await context.ProductVariants.AnyAsync())
            {
                return;
            }

            var products = await context.Products.ToListAsync();
            var variants = new List<ProductVariant>();

            var sizes = new[] { "XS", "S", "M", "L", "XL", "XXL" };
            var colors = new[] { "Black", "White", "Blue", "Red", "Navy", "Grey", "Green", "Purple" };

            // Create 3-4 variants for each product
            foreach (var product in products)
            {
                if (product.HasVariants)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        variants.Add(new ProductVariant
                        {
                            ProductId = product.Id,
                            Size = sizes[i % sizes.Length],
                            Color = colors[i % colors.Length],
                            ImageUrl = $"https://images.unsplash.com/photo-{1632564600 + (i * 100)}?w=500",
                            AdditionalPrice = i > 0 ? (decimal)(5.00 * i) : 0,
                            StockQuantity = 20 + (i * 10)
                        });
                    }
                }
            }

            await context.ProductVariants.AddRangeAsync(variants);
            await context.SaveChangesAsync();
        }
    }
}
