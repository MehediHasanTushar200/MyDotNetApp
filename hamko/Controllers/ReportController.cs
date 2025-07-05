using hamko.Models;
using hamko.Service;
using hamko.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace hamko.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ReportController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new StockReportViewModel
            {
                Products = _context.Products
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                    .ToList(),
                Branches = _context.Branches
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.BranchName })
                    .ToList(),
                FromDate = DateTime.Today.AddDays(-7),
                ToDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(StockReportViewModel model)
        {
            // Re-populate dropdowns
            model.Products = _context.Products
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList();
            model.Branches = _context.Branches
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.BranchName })
                .ToList();

            if (!model.FromDate.HasValue || !model.ToDate.HasValue)
            {
                ModelState.AddModelError("", "Please select both From and To dates.");
                return View(model);
            }

            var from = model.FromDate.Value.Date;
            var to = model.ToDate.Value.Date.AddDays(1).AddTicks(-1);

            // --- Purchase (StockIn) ---
            var purchaseQuery = _context.StockIns
                .Include(si => si.Product)
                .Include(si => si.Branch)
                .Include(si => si.Purchase)
                .Where(si =>
                    (!model.SelectedProductId.HasValue || si.ProductId == model.SelectedProductId.Value) &&
                    (!model.SelectedBranchId.HasValue || si.BranchId == model.SelectedBranchId.Value) &&
                    si.Purchase.Date >= from &&
                    si.Purchase.Date <= to
                );

            model.PurchaseResults = purchaseQuery
                .GroupBy(si => new { si.Product.Name, si.Price, si.BranchId })
                .Select(g => new StockReportRow
                {
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                    Price = g.Key.Price
                })
                .ToList();

            // --- Sales (StockOut) ---
            var salesQuery = _context.StockOuts
                .Include(so => so.Product)
                .Include(so => so.Branch)
                .Include(so => so.Sales)
                .Where(so =>
                    (!model.SelectedProductId.HasValue || so.ProductId == model.SelectedProductId.Value) &&
                    (!model.SelectedBranchId.HasValue || so.BranchId == model.SelectedBranchId.Value) &&
                    so.Sales.Date >= from &&
                    so.Sales.Date <= to
                );

            model.SalesResults = salesQuery
                .GroupBy(so => new { so.Product.Name, so.Price, so.BranchId })
                .Select(g => new StockReportRow
                {
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                    Price = g.Key.Price
                })
                .ToList();

            return View(model);
        }
    }
}
