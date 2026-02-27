using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public abstract class Entity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime DeletedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
    }
}