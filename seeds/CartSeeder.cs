using ECommerce.Models;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Seeds
{
    public static class CartSeeder
    {
        public static async Task SeedCartAsync(AppDbContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == "customer@example.com");
            if (user == null)
            {
                return;
            }

            // Check if cart already exists
            var existingCart = await context.Carts.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (existingCart != null)
            {
                return;
            }

            // Create cart for user
            var cart = new Cart
            {
                UserId = user.Id,
                User = user,
                Items = new List<CartItem>()
            };

            await context.Carts.AddAsync(cart);
            await context.SaveChangesAsync();

            // Get all products to ensure we have 30+ cart items
            var allProducts = await context.Products.Include(p => p.Variants).ToListAsync();
            var cartItems = new List<CartItem>();

            foreach (var product in allProducts)
            {
                if (product.HasVariants)
                {
                    var variant = await context.ProductVariants
                        .FirstOrDefaultAsync(v => v.ProductId == product.Id);

                    if (variant != null)
                    {
                        var unitPrice = product.HasDiscount ? product.PriceAfterDiscount : product.Price;
                        if (variant.AdditionalPrice.HasValue)
                        {
                            unitPrice += variant.AdditionalPrice.Value;
                        }

                        cartItems.Add(new CartItem
                        {
                            CartId = cart.Id,
                            ProductId = product.Id,
                            ProductVariantId = variant.Id,
                            Quantity = 1,
                            UnitPrice = unitPrice
                        });
                    }
                }
                else
                {
                    var unitPrice = product.HasDiscount ? product.PriceAfterDiscount : product.Price;

                    cartItems.Add(new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = product.Id,
                        Quantity = 1,
                        UnitPrice = unitPrice
                    });
                }
            }

            await context.CartItems.AddRangeAsync(cartItems);
            await context.SaveChangesAsync();
        }
    }
}
