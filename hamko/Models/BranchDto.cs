namespace hamko.Models
{
    public class BranchDto
    {
        public IFormFile? ImageFile { get; set; }
        public string BranchName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; }
    }
}
