using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Seeds;

public static class WishListSeeder
{
    public static async Task SeedWishListAsync(AppDbContext context)
    {
        if (await context.WishLists.AnyAsync())
        {
            return; // WishList already seeded
        }

        var customer = await context.Users.FirstOrDefaultAsync(u => u.Email == "customer@example.com");
        if (customer == null)
        {
            return; // Customer user not found
        }

        // Get all products to add to wishlist (35 products = 35 wishlist items)
        var allProducts = await context.Products.ToListAsync();
        var wishListItems = new List<WishList>();

        foreach (var product in allProducts)
        {
            wishListItems.Add(new WishList
            {
                UserId = customer.Id,
                ProductId = product.Id
            });
        }

        await context.WishLists.AddRangeAsync(wishListItems);
        await context.SaveChangesAsync();
    }
}
