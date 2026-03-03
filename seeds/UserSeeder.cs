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

            // Regular User
            var userEmail = "customer@example.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);

            if (regularUser == null)
            {
                regularUser = new ApplicationUser
                {
                    UserName = "customer",
                    Email = userEmail,
                    FullName = "John Doe",
                    EmailConfirmed = true,
                    PhoneNumber = "+9876543210",
                    Address = "456 Customer Avenue, Shopping Town, ST 54321",
                    ImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400"
                };

                var result = await userManager.CreateAsync(regularUser, "Customer@123");
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(userEmail);
                    user!.EmailConfirmed = true;
                    await userManager.UpdateAsync(user);
                    await userManager.AddToRoleAsync(regularUser, AppConstants.UserRole);

                }
            }
        }
    }
}
