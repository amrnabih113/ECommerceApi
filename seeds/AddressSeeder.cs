using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Seeds
{
    public static class AddressSeeder
    {
        public static async Task SeedAddressesAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            // Check if addresses already exist
            if (await context.Addresses.AnyAsync())
                return;

            var addresses = new List<Address>();

            // John Doe's addresses
            var johnDoe = await userManager.FindByEmailAsync("customer@example.com");
            if (johnDoe != null)
            {
                addresses.AddRange(new List<Address>
                {
                    new Address
                    {
                        UserId = johnDoe.Id,
                        Country = "United States",
                        City = "New York",
                        Street = "123 Main Street, Apt 4B",
                        PostalCode = "10001"
                    },
                    new Address
                    {
                        UserId = johnDoe.Id,
                        Country = "United States",
                        City = "Los Angeles",
                        Street = "456 Oak Avenue, Suite 200",
                        PostalCode = "90001"
                    },
                    new Address
                    {
                        UserId = johnDoe.Id,
                        Country = "United States",
                        City = "Chicago",
                        Street = "789 Elm Street",
                        PostalCode = "60601"
                    }
                });
            }

            // Jane Smith's addresses
            var janeSmith = await userManager.FindByEmailAsync("jane@example.com");
            if (janeSmith != null)
            {
                addresses.AddRange(new List<Address>
                {
                    new Address
                    {
                        UserId = janeSmith.Id,
                        Country = "United States",
                        City = "San Francisco",
                        Street = "100 Market Street, Unit 5",
                        PostalCode = "94103"
                    },
                    new Address
                    {
                        UserId = janeSmith.Id,
                        Country = "United States",
                        City = "Seattle",
                        Street = "200 Pine Street",
                        PostalCode = "98101"
                    }
                });
            }

            // Mike Johnson's addresses
            var mikeJohnson = await userManager.FindByEmailAsync("mike@example.com");
            if (mikeJohnson != null)
            {
                addresses.AddRange(new List<Address>
                {
                    new Address
                    {
                        UserId = mikeJohnson.Id,
                        Country = "United States",
                        City = "Boston",
                        Street = "300 Congress Street",
                        PostalCode = "02210"
                    },
                    new Address
                    {
                        UserId = mikeJohnson.Id,
                        Country = "United States",
                        City = "Miami",
                        Street = "400 Biscayne Boulevard",
                        PostalCode = "33132"
                    },
                    new Address
                    {
                        UserId = mikeJohnson.Id,
                        Country = "United States",
                        City = "Denver",
                        Street = "500 16th Street",
                        PostalCode = "80202"
                    }
                });
            }

            // Sarah Williams's addresses
            var sarahWilliams = await userManager.FindByEmailAsync("sarah@example.com");
            if (sarahWilliams != null)
            {
                addresses.AddRange(new List<Address>
                {
                    new Address
                    {
                        UserId = sarahWilliams.Id,
                        Country = "United States",
                        City = "Philadelphia",
                        Street = "600 Market Street",
                        PostalCode = "19106"
                    },
                    new Address
                    {
                        UserId = sarahWilliams.Id,
                        Country = "United States",
                        City = "Houston",
                        Street = "700 Louisiana Street",
                        PostalCode = "77002"
                    }
                });
            }

            if (addresses.Any())
            {
                await context.Addresses.AddRangeAsync(addresses);
                await context.SaveChangesAsync();
            }
        }
    }
}
