using hamko.Service;
using hamko.Models;

namespace hamko.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string ImageFileName { get; set; } = "";
        public string BranchName { get; set; } = "";
        public string Description { get; set; } = "";
        public  string Status { get; set; }



    }
}
