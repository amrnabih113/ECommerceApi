using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }


        public string? Address { get; set; }

        public string? ImageUrl { get; set; }
    }
}
