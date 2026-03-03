using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class ReviewSeeder
    {
        public static async Task SeedReviewsAsync(AppDbContext context)
        {
            if (await context.Reviews.AnyAsync())
            {
                return;
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "customer@example.com");
            if (user == null)
            {
                return;
            }

            var products = await context.Products.Take(30).ToListAsync();

            var reviewComments = new[]
            {
                "Excellent quality! Highly recommend this product.",
                "Very satisfied with my purchase. Great value for money.",
                "Perfect fit and great material. Will buy again!",
                "Disappointed with the quality. Not as described.",
                "Fantastic product! Exceeded my expectations.",
                "Good product but took a bit longer to arrive.",
                "The color faded after first wash. Not happy.",
                "Amazing! Best purchase I've made in months.",
                "Average product. Nothing special but works fine.",
                "Superb quality and fast shipping. 5 stars!",
                "Not what I expected. Returning this item.",
                "Outstanding! Exactly as advertised.",
                "Pretty good. Minor defect but acceptable.",
                "Love it! My whole family wants one now.",
                "Decent quality for the price.",
                "Could be better. Slightly disappointed.",
                "Fantastic! Fast delivery and great quality.",
                "Not bad. Would recommend to friends.",
                "Exceptional product! Worth every penny.",
                "Good but had some issues with sizing.",
                "Perfect! No complaints whatsoever.",
                "Really happy with this purchase.",
                "Quality is top-notch. Highly satisfied.",
                "Great product at an affordable price.",
                "Excellent customer service and product quality.",
                "Very pleased with my order.",
                "Best value for money in this category.",
                "Highly impressed with the craftsmanship.",
                "Cannot fault this product. Perfection!",
                "Will definitely purchase again from this brand."
            };

            var ratings = new[] { 5, 5, 5, 2, 5, 4, 2, 5, 3, 5, 1, 5, 3, 5, 3, 3, 5, 4, 5, 3, 5, 4, 5, 4, 5, 4, 5, 5, 5, 4 };

            var reviews = new List<Review>();

            for (int i = 0; i < Math.Min(products.Count, reviewComments.Length); i++)
            {
                reviews.Add(new Review
                {
                    ProductId = products[i].Id,
                    UserId = user.Id,
                    Rating = ratings[i],
                    Comment = reviewComments[i],
                    CreatedAt = DateTime.UtcNow.AddDays(-(30 - i))
                });
            }

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
        }
    }
}
