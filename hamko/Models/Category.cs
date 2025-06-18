namespace hamko.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public string Status { get; set; } = "";
        public ICollection<Product> Products { get; set; }

    }
}
