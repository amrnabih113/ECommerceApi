using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? FullName { get; set; }
        public string? Address { get; set; }
        [Url]
        public string? ImageUrl { get; set; }

        
        public ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
