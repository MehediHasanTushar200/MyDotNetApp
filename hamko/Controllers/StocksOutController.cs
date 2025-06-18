using hamko.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hamko.Controllers
{
    public class StocksOutController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public StocksOutController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var stockOuts = context.StockOuts
                .Include(s => s.Product)
                .Include(s => s.Branch)
                .Include(s => s.Sales)
                .AsEnumerable() // LINQ to Objects ব্যবহার করার জন্য
                .GroupBy(s => new
                {
                    s.SalesId,
                    s.ProductId,
                    s.BranchId,
                    s.Quantity,
                    s.Price,
                    s.TotalPrice
                })
                .Select(g => g.First()) // প্রতি গ্রুপে প্রথম রেকর্ডটাই নিবে
                .ToList();

            return View(stockOuts);
        }

    }
}
