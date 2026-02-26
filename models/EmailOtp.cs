using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Models;

namespace ECommerce.models
{
    public class EmailOtp
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Code { get; set; }
        public DateTime ExpiryDate { get; set; }

        public ApplicationUser? User { get; set; }
    }
}
