
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class ProductImage : Entity
    {
        [Required]
        [Url]
        public string ImageUrl { get; set; } = null!;
        [Required]
        public bool IsMain { get; set; }

        [ForeignKey(nameof(ProductId))]
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}