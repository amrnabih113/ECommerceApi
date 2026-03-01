using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.DTOs.Products
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}