using System.ComponentModel.DataAnnotations;

namespace hamko.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!; // Non-nullable with default value

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Status { get; set; } = null!;

        [Required]
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!; // Non-nullable with default value

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!; // Non-nullable with default value

        public string? ImageFileName { get; set; } // Nullable since it's optional
    }
}
