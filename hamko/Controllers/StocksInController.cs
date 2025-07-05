using hamko.Models;
using hamko.Service;
using hamko.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hamko.Controllers
{
    public class StocksInController : Controller
    {
        private readonly ApplicationDbContext context;

        public StocksInController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new StockInSearchViewModel
            {
                Products = context.Products.ToList(),
                Branches = context.Branches.ToList(),
                Results = context.StockIns
                    .Include(s => s.Product)
                    .Include(s => s.Branch)
                    .ToList()
                    .GroupBy(s => new { s.ProductId, s.Product.Name })
                    .Select(g => new StockInGroupedViewModel
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.Name,
                        Quantity = g.Sum(x => x.Quantity),
                        Price = g.First().Price,
                        TotalPrice = g.Sum(x => x.Quantity * x.Price)
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(StockInSearchViewModel model)
        {
            var query = context.StockIns
                .Include(s => s.Product)
                .Include(s => s.Branch)
                .AsQueryable();

            if (model.SelectedProductId.HasValue)
                query = query.Where(s => s.ProductId == model.SelectedProductId);

            if (model.SelectedBranchId.HasValue)
                query = query.Where(s => s.BranchId == model.SelectedBranchId);

            model.Results = query
                .ToList()
                .GroupBy(s => new { s.ProductId, s.Product.Name })
                .Select(g => new StockInGroupedViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                    Price = g.First().Price,
                    TotalPrice = g.Sum(x => x.Quantity * x.Price)
                }).ToList();

            // Always rebind products and branches
            model.Products = context.Products.ToList();
            model.Branches = context.Branches.ToList();

            return View(model);
        }
    }
}
