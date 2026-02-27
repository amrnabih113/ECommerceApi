
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class CartItem : Entity
    {

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; set; } = null!;
    }
}