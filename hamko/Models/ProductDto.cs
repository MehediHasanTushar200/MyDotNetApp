using System.ComponentModel.DataAnnotations;

namespace hamko.Models
{
    public class ProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = null!;

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ImageFileName { get; internal set; }
    }
}

