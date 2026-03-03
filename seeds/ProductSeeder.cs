using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class ProductSeeder
    {
        public static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync())
            {
                return;
            }

            // Get categories and brands
            var tshirts = await context.Categories.FirstAsync(c => c.Name == "T-Shirts");
            var dresses = await context.Categories.FirstAsync(c => c.Name == "Dresses");
            var casualShoes = await context.Categories.FirstAsync(c => c.Name == "Casual Shoes");
            var sneakers = await context.Categories.FirstAsync(c => c.Name == "Sneakers");
            var jeans = await context.Categories.FirstAsync(c => c.Name == "Jeans");
            var shirts = await context.Categories.FirstAsync(c => c.Name == "Shirts");
            var hoodies = await context.Categories.FirstAsync(c => c.Name == "Hoodies & Sweatshirts");
            var sportShoes = await context.Categories.FirstAsync(c => c.Name == "Sport Shoes");

            var nike = await context.Brands.FirstAsync(b => b.Name == "Nike");
            var adidas = await context.Brands.FirstAsync(b => b.Name == "Adidas");
            var puma = await context.Brands.FirstAsync(b => b.Name == "Puma");
            var levis = await context.Brands.FirstAsync(b => b.Name == "Levi's");
            var hm = await context.Brands.FirstAsync(b => b.Name == "H&M");
            var zara = await context.Brands.FirstAsync(b => b.Name == "Zara");
            var gucci = await context.Brands.FirstAsync(b => b.Name == "Gucci");
            var tommy = await context.Brands.FirstAsync(b => b.Name == "Tommy Hilfiger");
            var ck = await context.Brands.FirstAsync(b => b.Name == "Calvin Klein");
            var uniqlo = await context.Brands.FirstAsync(b => b.Name == "Uniqlo");

            var products = new List<Product>
            {
                // Nike Products (4)
                new Product { Name = "Nike Classic Fit T-Shirt", Description = "Comfortable cotton blend t-shirt. Features Nike's signature swoosh logo.", Price = 29.99m, HasDiscount = true, PriceAfterDiscount = 19.99m, DiscountPercentage = 33, HasVariants = true, StockQuantity = 150, IsActive = true, CategoryId = tshirts.Id, BrandId = nike.Id },
                new Product { Name = "Nike Air Max Sneakers", Description = "Iconic Air Max sneakers with premium cushioning and contemporary design.", Price = 119.99m, HasDiscount = false, PriceAfterDiscount = 119.99m, HasVariants = true, StockQuantity = 80, IsActive = true, CategoryId = sneakers.Id, BrandId = nike.Id },
                new Product { Name = "Nike Running Shorts", Description = "Lightweight running shorts with moisture-wicking technology.", Price = 44.99m, HasDiscount = true, PriceAfterDiscount = 34.99m, DiscountPercentage = 22, HasVariants = true, StockQuantity = 95, IsActive = true, CategoryId = hoodies.Id, BrandId = nike.Id },
                new Product { Name = "Nike Dri-Fit Hoodie", Description = "Breathable hoodie with signature Nike design. Perfect for workouts.", Price = 59.99m, HasDiscount = false, PriceAfterDiscount = 59.99m, HasVariants = true, StockQuantity = 70, IsActive = true, CategoryId = hoodies.Id, BrandId = nike.Id },

                // Adidas Products (4)
                new Product { Name = "Adidas Performance T-Shirt", Description = "Moisture-wicking performance t-shirt designed for active athletes.", Price = 34.99m, HasDiscount = true, PriceAfterDiscount = 24.99m, DiscountPercentage = 29, HasVariants = true, StockQuantity = 120, IsActive = true, CategoryId = tshirts.Id, BrandId = adidas.Id },
                new Product { Name = "Adidas Ultraboost Shoes", Description = "Premium running shoes with boost technology for exceptional comfort.", Price = 179.99m, HasDiscount = true, PriceAfterDiscount = 139.99m, DiscountPercentage = 22, HasVariants = true, StockQuantity = 60, IsActive = true, CategoryId = sneakers.Id, BrandId = adidas.Id },
                new Product { Name = "Adidas Track Pants", Description = "Classic track pants with Adidas stripe design. Comfortable fit.", Price = 54.99m, HasDiscount = true, PriceAfterDiscount = 39.99m, DiscountPercentage = 27, HasVariants = true, StockQuantity = 85, IsActive = true, CategoryId = jeans.Id, BrandId = adidas.Id },
                new Product { Name = "Adidas Sweatshirt", Description = "Cozy sweatshirt perfect for casual wear and comfort.", Price = 49.99m, HasDiscount = false, PriceAfterDiscount = 49.99m, HasVariants = true, StockQuantity = 100, IsActive = true, CategoryId = hoodies.Id, BrandId = adidas.Id },

                // Levi's Products (4)
                new Product { Name = "Levi's 501 Original Fit Jeans", Description = "Iconic straight-fit jeans. A staple since 1873. Premium denim quality.", Price = 79.99m, HasDiscount = false, PriceAfterDiscount = 79.99m, HasVariants = true, StockQuantity = 100, IsActive = true, CategoryId = jeans.Id, BrandId = levis.Id },
                new Product { Name = "Levi's Casual Canvas Shoes", Description = "Versatile canvas shoes for casual everyday wear. Comfortable and durable.", Price = 49.99m, HasDiscount = true, PriceAfterDiscount = 39.99m, DiscountPercentage = 20, HasVariants = true, StockQuantity = 90, IsActive = true, CategoryId = casualShoes.Id, BrandId = levis.Id },
                new Product { Name = "Levi's Slim Fit Jeans", Description = "Modern slim fit jeans with premium dapple dye wash.", Price = 74.99m, HasDiscount = true, PriceAfterDiscount = 59.99m, DiscountPercentage = 20, HasVariants = true, StockQuantity = 75, IsActive = true, CategoryId = jeans.Id, BrandId = levis.Id },
                new Product { Name = "Levi's Polo Shirt", Description = "Classic polo shirt in premium cotton. Timeless style.", Price = 44.99m, HasDiscount = false, PriceAfterDiscount = 44.99m, HasVariants = true, StockQuantity = 110, IsActive = true, CategoryId = shirts.Id, BrandId = levis.Id },

                // H&M Products (4)
                new Product { Name = "H&M Striped Summer Dress", Description = "Light and breathable striped dress perfect for summer. Stylish and comfortable.", Price = 39.99m, HasDiscount = true, PriceAfterDiscount = 27.99m, DiscountPercentage = 30, HasVariants = true, StockQuantity = 110, IsActive = true, CategoryId = dresses.Id, BrandId = hm.Id },
                new Product { Name = "H&M Basic T-Shirt Collection", Description = "Essential basic t-shirt in multiple colors. Perfect for layering or standalone wear.", Price = 14.99m, HasDiscount = false, PriceAfterDiscount = 14.99m, HasVariants = true, StockQuantity = 200, IsActive = true, CategoryId = tshirts.Id, BrandId = hm.Id },
                new Product { Name = "H&M Linen Shirts", Description = "Breathable linen shirts perfect for warm weather.", Price = 34.99m, HasDiscount = true, PriceAfterDiscount = 24.99m, DiscountPercentage = 29, HasVariants = true, StockQuantity = 80, IsActive = true, CategoryId = shirts.Id, BrandId = hm.Id },
                new Product { Name = "H&M Joggers", Description = "Comfortable joggers for casual wear. Modern fit and style.", Price = 39.99m, HasDiscount = true, PriceAfterDiscount = 29.99m, DiscountPercentage = 25, HasVariants = true, StockQuantity = 95, IsActive = true, CategoryId = jeans.Id, BrandId = hm.Id },

                // Zara Products (4)
                new Product { Name = "Zara Contemporary Dress", Description = "Modern and elegant dress with sophisticated design. Perfect for any occasion.", Price = 59.99m, HasDiscount = false, PriceAfterDiscount = 59.99m, HasVariants = true, StockQuantity = 75, IsActive = true, CategoryId = dresses.Id, BrandId = zara.Id },
                new Product { Name = "Zara Premium White Shirt", Description = "Crisp and clean white shirt made from premium materials. A wardrobe essential.", Price = 49.99m, HasDiscount = true, PriceAfterDiscount = 34.99m, DiscountPercentage = 30, HasVariants = true, StockQuantity = 85, IsActive = true, CategoryId = shirts.Id, BrandId = zara.Id },
                new Product { Name = "Zara Black Blazer", Description = "Elegant black blazer for professional and casual wear.", Price = 89.99m, HasDiscount = true, PriceAfterDiscount = 64.99m, DiscountPercentage = 28, HasVariants = true, StockQuantity = 60, IsActive = true, CategoryId = hoodies.Id, BrandId = zara.Id },
                new Product { Name = "Zara Floral Dress", Description = "Beautiful floral patterned dress. Lightweight and comfortable.", Price = 54.99m, HasDiscount = false, PriceAfterDiscount = 54.99m, HasVariants = true, StockQuantity = 70, IsActive = true, CategoryId = dresses.Id, BrandId = zara.Id },

                // Puma Products (3)
                new Product { Name = "Puma Running Shoes", Description = "Lightweight running shoes with responsive cushioning technology.", Price = 99.99m, HasDiscount = true, PriceAfterDiscount = 74.99m, DiscountPercentage = 25, HasVariants = true, StockQuantity = 70, IsActive = true, CategoryId = sportShoes.Id, BrandId = puma.Id },
                new Product { Name = "Puma Sports T-Shirt", Description = "Breathable sports t-shirt with moisture-wicking technology.", Price = 24.99m, HasDiscount = false, PriceAfterDiscount = 24.99m, HasVariants = true, StockQuantity = 130, IsActive = true, CategoryId = tshirts.Id, BrandId = puma.Id },
                new Product { Name = "Puma Training Shorts", Description = "Quality training shorts for active sports activities.", Price = 34.99m, HasDiscount = true, PriceAfterDiscount = 24.99m, DiscountPercentage = 29, HasVariants = true, StockQuantity = 100, IsActive = true, CategoryId = jeans.Id, BrandId = puma.Id },

                // Gucci Products (3)
                new Product { Name = "Gucci Premium T-Shirt", Description = "Luxury t-shirt with iconic Gucci branding and premium materials.", Price = 189.99m, HasDiscount = false, PriceAfterDiscount = 189.99m, HasVariants = true, StockQuantity = 40, IsActive = true, CategoryId = tshirts.Id, BrandId = gucci.Id },
                new Product { Name = "Gucci Signature Shoes", Description = "Elegant shoes with signature Gucci design elements.", Price = 249.99m, HasDiscount = true, PriceAfterDiscount = 199.99m, DiscountPercentage = 20, HasVariants = true, StockQuantity = 35, IsActive = true, CategoryId = casualShoes.Id, BrandId = gucci.Id },
                new Product { Name = "Gucci Cotton Shirt", Description = "Premium cotton shirt with subtle luxury detailing.", Price = 179.99m, HasDiscount = false, PriceAfterDiscount = 179.99m, HasVariants = true, StockQuantity = 45, IsActive = true, CategoryId = shirts.Id, BrandId = gucci.Id },

                // Tommy Hilfiger Products (3)
                new Product { Name = "Tommy Hilfiger Classic T-Shirt", Description = "Classic Tommy Hilfiger t-shirt with iconic stripe.", Price = 39.99m, HasDiscount = true, PriceAfterDiscount = 29.99m, DiscountPercentage = 25, HasVariants = true, StockQuantity = 120, IsActive = true, CategoryId = tshirts.Id, BrandId = tommy.Id },
                new Product { Name = "Tommy Hilfiger Polo", Description = "Traditional polo shirt with embroidered flag logo.", Price = 54.99m, HasDiscount = false, PriceAfterDiscount = 54.99m, HasVariants = true, StockQuantity = 90, IsActive = true, CategoryId = shirts.Id, BrandId = tommy.Id },
                new Product { Name = "Tommy Hilfiger Shorts", Description = "Comfortable shorts with classic Tommy style.", Price = 44.99m, HasDiscount = true, PriceAfterDiscount = 34.99m, DiscountPercentage = 22, HasVariants = true, StockQuantity = 85, IsActive = true, CategoryId = jeans.Id, BrandId = tommy.Id },

                // Calvin Klein Products (3)
                new Product { Name = "Calvin Klein Minimalist T-Shirt", Description = "Clean minimalist t-shirt with premium quality.", Price = 44.99m, HasDiscount = false, PriceAfterDiscount = 44.99m, HasVariants = true, StockQuantity = 115, IsActive = true, CategoryId = tshirts.Id, BrandId = ck.Id },
                new Product { Name = "Calvin Klein Jeans", Description = "Slim fit jeans with timeless Calvin Klein design.", Price = 79.99m, HasDiscount = true, PriceAfterDiscount = 59.99m, DiscountPercentage = 25, HasVariants = true, StockQuantity = 70, IsActive = true, CategoryId = jeans.Id, BrandId = ck.Id },
                new Product { Name = "Calvin Klein Performance Hoodie", Description = "Sporty hoodie with performance fabric technology.", Price = 69.99m, HasDiscount = false, PriceAfterDiscount = 69.99m, HasVariants = true, StockQuantity = 65, IsActive = true, CategoryId = hoodies.Id, BrandId = ck.Id },

                // Uniqlo Products (3)
                new Product { Name = "Uniqlo Ultra Soft T-Shirt", Description = "Super soft cotton t-shirt for everyday comfort.", Price = 9.99m, HasDiscount = false, PriceAfterDiscount = 9.99m, HasVariants = true, StockQuantity = 250, IsActive = true, CategoryId = tshirts.Id, BrandId = uniqlo.Id },
                new Product { Name = "Uniqlo Heattech Long Sleeve", Description = "Thermal heattech shirt for warmth and comfort.", Price = 19.99m, HasDiscount = true, PriceAfterDiscount = 14.99m, DiscountPercentage = 25, HasVariants = true, StockQuantity = 180, IsActive = true, CategoryId = shirts.Id, BrandId = uniqlo.Id },
                new Product { Name = "Uniqlo Easy Care Jeans", Description = "Low maintenance jeans that are wrinkle resistant.", Price = 39.99m, HasDiscount = false, PriceAfterDiscount = 39.99m, HasVariants = true, StockQuantity = 140, IsActive = true, CategoryId = jeans.Id, BrandId = uniqlo.Id }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
