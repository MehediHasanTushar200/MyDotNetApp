namespace hamko.ViewModels
{
    public class StockReportRow
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public string BranchName { get; set; }
        // Read-only computed property
        public decimal TotalPrice => Quantity * Price;
    }
}
