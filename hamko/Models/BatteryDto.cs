namespace hamko.Models
{
    public class BatteryDto
    {
        public string Name { get; set; } = "";
        public IFormFile? ImageFile { get; set; }
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; }

    }
}
