using hamko.Models;

namespace hamko.ViewModels
{
    public class StockInGroupedViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class StockInSearchViewModel
    {
        public int? SelectedProductId { get; set; }
        public int? SelectedBranchId { get; set; }

        public List<StockInGroupedViewModel> Results { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<Branch> Branches { get; set; } = new();
    }
}
