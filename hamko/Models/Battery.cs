namespace hamko.Models
{
    public class Battery
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "Active"; // Default value for Status
    }
}
