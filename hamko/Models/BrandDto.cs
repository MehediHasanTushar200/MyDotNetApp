namespace hamko.Models
{
    public class BrandDto
    {
        public string Name { get; set; } = "";
        public IFormFile? ImageFile { get; set; }
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public ICollection<Product> Products { get; set; }


    }
}
