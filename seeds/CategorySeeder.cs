using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class CategorySeeder
    {
        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            var categories = new List<Category>
            {
                // Parent Categories
                new Category { Name = "Men's Fashion", Description = "Clothing and accessories for men", ImageUrl = "https://images.unsplash.com/photo-1552126613-52172884d531?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Women's Fashion", Description = "Clothing and accessories for women", ImageUrl = "https://images.unsplash.com/photo-1487180144351-b8472da7d491?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Footwear", Description = "Shoes, sneakers, and footwear for all genders", ImageUrl = "https://images.unsplash.com/photo-1460353581641-37baddab0fa2?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Accessories", Description = "Bags, belts, scarves, and other accessories", ImageUrl = "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Sports & Active", Description = "Athletic wear and sports equipment", ImageUrl = "https://images.unsplash.com/photo-1464207687429-7505649dae38?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Outerwear", Description = "Coats, jackets, and winter wear", ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16ebc5?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Sleepwear & Loungewear", Description = "Comfortable home and sleep apparel", ImageUrl = "https://images.unsplash.com/photo-1599643478518-a784e5dc4c8f?w=400", IsActive = true, ParentCategoryId = null },
                new Category { Name = "Swimwear", Description = "Beachwear and swimwear collection", ImageUrl = "https://images.unsplash.com/photo-1612359063403-4ee6f6230e81?w=400", IsActive = true, ParentCategoryId = null }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Get parent categories
            var menFashion = await context.Categories.FirstAsync(c => c.Name == "Men's Fashion");
            var womenFashion = await context.Categories.FirstAsync(c => c.Name == "Women's Fashion");
            var footwear = await context.Categories.FirstAsync(c => c.Name == "Footwear");
            var accessories = await context.Categories.FirstAsync(c => c.Name == "Accessories");
            var sports = await context.Categories.FirstAsync(c => c.Name == "Sports & Active");
            var outerwear = await context.Categories.FirstAsync(c => c.Name == "Outerwear");

            var subCategories = new List<Category>
            {
                // Men's Fashion - 5 subcategories
                new Category { Name = "T-Shirts", Description = "Casual and formal t-shirts", ImageUrl = "https://images.unsplash.com/photo-1553743760-d2bebdc1440a?w=400", IsActive = true, ParentCategoryId = menFashion.Id },
                new Category { Name = "Jeans", Description = "Classic and trendy jeans", ImageUrl = "https://images.unsplash.com/photo-1542272604-787c62228ecb?w=400", IsActive = true, ParentCategoryId = menFashion.Id },
                new Category { Name = "Shirts", Description = "Formal and casual shirts", ImageUrl = "https://images.unsplash.com/photo-1490362891991-f776e74a9a3f?w=400", IsActive = true, ParentCategoryId = menFashion.Id },
                new Category { Name = "Pants", Description = "Dress and casual pants", ImageUrl = "https://images.unsplash.com/photo-1505048338325-6f814f5565cc?w=400", IsActive = true, ParentCategoryId = menFashion.Id },
                new Category { Name = "Hoodies & Sweatshirts", Description = "Comfortable hoodies and sweatshirts", ImageUrl = "https://images.unsplash.com/photo-1556821552-5d63b397dfd1?w=400", IsActive = true, ParentCategoryId = menFashion.Id },
                
                // Women's Fashion - 5 subcategories
                new Category { Name = "Dresses", Description = "Casual and formal dresses", ImageUrl = "https://images.unsplash.com/photo-1595777707802-04b3cfd9b2c4?w=400", IsActive = true, ParentCategoryId = womenFashion.Id },
                new Category { Name = "Tops & Blouses", Description = "Shirts and blouses", ImageUrl = "https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=400", IsActive = true, ParentCategoryId = womenFashion.Id },
                new Category { Name = "Skirts & Pants", Description = "Skirts and trousers", ImageUrl = "https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=400", IsActive = true, ParentCategoryId = womenFashion.Id },
                new Category { Name = "Leggings", Description = "Comfortable stretchy leggings", ImageUrl = "https://images.unsplash.com/photo-1506259926059-2aded27c532f?w=400", IsActive = true, ParentCategoryId = womenFashion.Id },
                new Category { Name = "Women's Hoodies", Description = "Hoodies and sweatshirts for women", ImageUrl = "https://images.unsplash.com/photo-1556821552-f06b0073f237?w=400", IsActive = true, ParentCategoryId = womenFashion.Id },
                
                // Footwear - 4 subcategories
                new Category { Name = "Casual Shoes", Description = "Everyday casual shoes", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", IsActive = true, ParentCategoryId = footwear.Id },
                new Category { Name = "Sneakers", Description = "Modern sneakers", ImageUrl = "https://images.unsplash.com/photo-1459262838948-3e2de6c3ea36?w=400", IsActive = true, ParentCategoryId = footwear.Id },
                new Category { Name = "Sport Shoes", Description = "Athletic footwear", ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", IsActive = true, ParentCategoryId = footwear.Id },
                new Category { Name = "Sandals & Flip Flops", Description = "Summer and casual sandals", ImageUrl = "https://images.unsplash.com/photo-1551028719-00167b16ebc5?w=400", IsActive = true, ParentCategoryId = footwear.Id },
                
                // Accessories - 4 subcategories
                new Category { Name = "Bags & Backpacks", Description = "Handbags and backpacks", ImageUrl = "https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=400", IsActive = true, ParentCategoryId = accessories.Id },
                new Category { Name = "Belts", Description = "Fashion and casual belts", ImageUrl = "https://images.unsplash.com/photo-1550777662-c869b62908e1?w=400", IsActive = true, ParentCategoryId = accessories.Id },
                new Category { Name = "Scarves & Shawls", Description = "Scarves and wraps", ImageUrl = "https://images.unsplash.com/photo-1571415527789-4b4f0e92f215?w=400", IsActive = true, ParentCategoryId = accessories.Id },
                new Category { Name = "Hats & Caps", Description = "Hats, caps, and beanies", ImageUrl = "https://images.unsplash.com/photo-1551743760-d2bebdc1440a?w=400", IsActive = true, ParentCategoryId = accessories.Id },
                
                // Sports & Active - 3 subcategories
                new Category { Name = "Gym Wear", Description = "Activewear for gym", ImageUrl = "https://images.unsplash.com/photo-1506259926059-2aded27c532f?w=400", IsActive = true, ParentCategoryId = sports.Id },
                new Category { Name = "Running Gear", Description = "Running and jogging wear", ImageUrl = "https://images.unsplash.com/photo-1506259926059-2aded27c532f?w=400", IsActive = true, ParentCategoryId = sports.Id },
                new Category { Name = "Yoga & Pilates", Description = "Yoga and pilates apparel", ImageUrl = "https://images.unsplash.com/photo-1509899636127-2a00e872b895?w=400", IsActive = true, ParentCategoryId = sports.Id },
                
                // Outerwear - 3 subcategories
                new Category { Name = "Jackets", Description = "All types of jackets", ImageUrl = "https://images.unsplash.com/photo-1492849217124-91e7a266ff0d?w=400", IsActive = true, ParentCategoryId = outerwear.Id },
                new Category { Name = "Coats", Description = "Winter and formal coats", ImageUrl = "https://images.unsplash.com/photo-1539533057144-f587b92e31e2?w=400", IsActive = true, ParentCategoryId = outerwear.Id },
                new Category { Name = "Vests", Description = "Sleeveless vests and waistcoats", ImageUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=400", IsActive = true, ParentCategoryId = outerwear.Id }};
            await context.Categories.AddRangeAsync(subCategories);
            await context.SaveChangesAsync();
        }
    }
}
