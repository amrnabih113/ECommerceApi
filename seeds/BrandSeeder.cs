using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class BrandSeeder
    {
        public static async Task SeedBrandsAsync(AppDbContext context)
        {
            if (await context.Brands.AnyAsync())
            {
                return;
            }

            var brands = new List<Brand>
            {
                new Brand { Name = "Nike", Description = "Leading sports and athletic apparel brand known for innovation and performance.", LogoUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400" },
                new Brand { Name = "Adidas", Description = "Global sportswear manufacturer offering footwear, apparel, and accessories.", LogoUrl = "https://images.unsplash.com/photo-1556821552-5d63b397dfd1?w=400" },
                new Brand { Name = "Puma", Description = "Sports brand specializing in athletic shoes, apparel, and equipment.", LogoUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400" },
                new Brand { Name = "Levi's", Description = "Premium denim and casual wear brand with over 170 years of heritage.", LogoUrl = "https://images.unsplash.com/photo-1506157786151-b8491531f063?w=400" },
                new Brand { Name = "H&M", Description = "Fast fashion retailer offering trendy and affordable clothing for all ages.", LogoUrl = "https://images.unsplash.com/photo-1555685812-4b943f1cb0eb?w=400" },
                new Brand { Name = "Zara", Description = "Contemporary fashion brand known for innovative designs and quick-to-market collections.", LogoUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400" },
                new Brand { Name = "Gucci", Description = "Luxury Italian fashion brand renowned for high-quality and exclusive designs.", LogoUrl = "https://images.unsplash.com/photo-1584487990133-d3a5ad424715?w=400" },
                new Brand { Name = "Louis Vuitton", Description = "Prestigious luxury fashion brand famous for leather goods and accessories.", LogoUrl = "https://images.unsplash.com/photo-1578062867171-2a5f03b7bae8?w=400" },
                new Brand { Name = "Tommy Hilfiger", Description = "American fashion brand offering casual wear and accessories with classic styling.", LogoUrl = "https://images.unsplash.com/photo-1524224097839-896541edbbfe?w=400" },
                new Brand { Name = "Calvin Klein", Description = "Premium fashion brand known for minimalist designs and quality materials.", LogoUrl = "https://images.unsplash.com/photo-1515564612207-3775acd4835e?w=400" },
                new Brand { Name = "Versace", Description = "Italian luxury fashion brand known for bold designs and vibrant colors.", LogoUrl = "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=400" },
                new Brand { Name = "Burberry", Description = "British luxury brand famous for classic checks and premium outerwear.", LogoUrl = "https://images.unsplash.com/photo-1539533057144-f587b92e31e2?w=400" },
                new Brand { Name = "Prada", Description = "Italian luxury fashion house known for minimalist elegance and quality.", LogoUrl = "https://images.unsplash.com/photo-1491553895911-0055eca6402d?w=400" },
                new Brand { Name = "Chanel", Description = "Iconic French luxury brand renowned for timeless elegance and style.", LogoUrl = "https://images.unsplash.com/photo-1523170335258-f875a1eca10e?w=400" },
                new Brand { Name = "Fendi", Description = "Italian luxury brand known for innovative designs and fine craftsmanship.", LogoUrl = "https://images.unsplash.com/photo-1523293182086-7651a899d37f?w=400" },
                new Brand { Name = "Coach", Description = "American luxury brand offering high-quality bags and accessories.", LogoUrl = "https://images.unsplash.com/photo-1548036328-c9fa89d128fa?w=400" },
                new Brand { Name = "Ralph Lauren", Description = "American fashion brand known for classic preppy style and quality.", LogoUrl = "https://images.unsplash.com/photo-1505628346881-b72b27e84530?w=400" },
                new Brand { Name = "Gap", Description = "Casual American fashion retailer offering versatile everyday wear.", LogoUrl = "https://images.unsplash.com/photo-1459262838948-3e2de6c3ea36?w=400" },
                new Brand { Name = "Uniqlo", Description = "Japanese brand specializing in simple, quality, and timeless basics.", LogoUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400" },
                new Brand { Name = "Forever 21", Description = "Fast fashion brand offering trendy and budget-friendly clothing.", LogoUrl = "https://images.unsplash.com/photo-1595521622507-cf1a9419bad5?w=400" },
                new Brand { Name = "ASOS", Description = "Online fashion retailer with diverse styles and trending pieces.", LogoUrl = "https://images.unsplash.com/photo-1490481651871-ab68de25d43d?w=400" },
                new Brand { Name = "Mango", Description = "Contemporary fashion brand known for modern and sophisticated designs.", LogoUrl = "https://images.unsplash.com/photo-1551995260-76cfdf56ae38?w=400" },
                new Brand { Name = "Banana Republic", Description = "Premium American brand offering sophisticated and polished apparel.", LogoUrl = "https://images.unsplash.com/photo-1539533057144-f587b92e31e2?w=400" },
                new Brand { Name = "Ann Taylor", Description = "Women's fashion brand specializing in professional and casual wear.", LogoUrl = "https://images.unsplash.com/photo-1595521622507-cf1a9419bad5?w=400" },
                new Brand { Name = "Express", Description = "Fashion brand offering contemporary styles and premium basics.", LogoUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400" },
                new Brand { Name = "J.Crew", Description = "American fashion brand known for quality basics and timeless style.", LogoUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400" },
                new Brand { Name = "Dockers", Description = "Casual apparel brand specializing in comfortable khakis and basics.", LogoUrl = "https://images.unsplash.com/photo-1516762689617-e1cffcef541d?w=400" },
                new Brand { Name = "Timberland", Description = "Outdoor and casual wear brand known for quality boots and apparel.", LogoUrl = "https://images.unsplash.com/photo-1460353581641-37baddab0fa2?w=400" },
                new Brand { Name = "The North Face", Description = "Outdoor brand specializing in adventure gear and performance wear.", LogoUrl = "https://images.unsplash.com/photo-1492849217124-91e7a266ff0d?w=400" },
                new Brand { Name = "Columbia", Description = "Outdoor apparel brand offering weather-resistant clothing and gear.", LogoUrl = "https://images.unsplash.com/photo-1539533057144-f587b92e31e2?w=400" },
                new Brand { Name = "Patagonia", Description = "Premium outdoor brand committed to sustainability and quality.", LogoUrl = "https://images.unsplash.com/photo-1516238323070-a54ee2c4a56f?w=400" },
                new Brand { Name = "Skechers", Description = "Casual footwear brand known for comfortable and stylish shoes.", LogoUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400" },
                new Brand { Name = "New Balance", Description = "Athletic footwear brand known for quality running and lifestyle shoes.", LogoUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400" } };
            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();
        }
    }
}
