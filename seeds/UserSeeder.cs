using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using ECommerce.core.utils;

namespace ECommerce.Seeds
{
    public static class UserSeeder
    {
        public static async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            // Admin User
            var adminEmail = "admin@ecommerce.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "Admin User",
                    EmailConfirmed = true,
                    PhoneNumber = "+1234567890",
                    Address = "123 Admin Street, Tech City, TC 12345",
                    ImageUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AppConstants.AdminRole);
                }
            }

            // Customer 1 - John Doe
            var customer1Email = "customer@example.com";
            var customer1 = await userManager.FindByEmailAsync(customer1Email);

            if (customer1 == null)
            {
                customer1 = new ApplicationUser
                {
                    UserName = "customer",
                    Email = customer1Email,
                    FullName = "John Doe",
                    EmailConfirmed = true,
                    PhoneNumber = "+9876543210",
                    Address = "456 Customer Avenue, Shopping Town, ST 54321",
                    ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400"
                };

                var result = await userManager.CreateAsync(customer1, "Customer@123");
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(customer1Email);
                    user!.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(customer1, AppConstants.UserRole);
                }
            }

            // Customer 2 - Jane Smith
            var customer2Email = "jane@example.com";
            var customer2 = await userManager.FindByEmailAsync(customer2Email);

            if (customer2 == null)
            {
                customer2 = new ApplicationUser
                {
                    UserName = "jane_smith",
                    Email = customer2Email,
                    FullName = "Jane Smith",
                    EmailConfirmed = true,
                    PhoneNumber = "+1122334455",
                    Address = "789 Elm Street, Downtown Plaza, DP 98765",
                    ImageUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400"
                };

                var result = await userManager.CreateAsync(customer2, "JaneSmith@123");
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(customer2Email);
                    user!.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(customer2, AppConstants.UserRole);
                }
            }

            // Customer 3 - Mike Johnson
            var customer3Email = "mike@example.com";
            var customer3 = await userManager.FindByEmailAsync(customer3Email);

            if (customer3 == null)
            {
                customer3 = new ApplicationUser
                {
                    UserName = "mike_johnson",
                    Email = customer3Email,
                    FullName = "Mike Johnson",
                    EmailConfirmed = true,
                    PhoneNumber = "+5544332211",
                    Address = "321 Oak Lane, Tech Park, TP 55555",
                    ImageUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400"
                };

                var result = await userManager.CreateAsync(customer3, "MikeJohnson@123");
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(customer3Email);
                    user!.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(customer3, AppConstants.UserRole);
                }
            }

            // Customer 4 - Sarah Williams
            var customer4Email = "sarah@example.com";
            var customer4 = await userManager.FindByEmailAsync(customer4Email);

            if (customer4 == null)
            {
                customer4 = new ApplicationUser
                {
                    UserName = "sarah_williams",
                    Email = customer4Email,
                    FullName = "Sarah Williams",
                    EmailConfirmed = true,
                    PhoneNumber = "+6677889900",
                    Address = "654 Pine Road, Shopping District, SD 77777",
                    ImageUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400"
                };

                var result = await userManager.CreateAsync(customer4, "SarahWilliams@123");
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(customer4Email);
                    user!.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(customer4, AppConstants.UserRole);
                }
            }
        }
    }
}
