using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.DTOs.Brands
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public required string LogoUrl { get; set; }

        public int ProductsCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}