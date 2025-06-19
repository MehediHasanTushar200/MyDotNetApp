using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hamko.Controllers
{
    public class SalesController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SalesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
    
        public IActionResult Index()
        {
            var sales = _context.Sales.ToList();
            return View(sales);
        }

           public IActionResult Create()
    {
        var model = new Sales
        {
            Date = DateTime.Now,
            InvoiceNo = GenerateInvoiceNumber(),
            RefNo = GenerateRefNumber(),
            StockOuts = new List<StockOut> { new StockOut() }
        };

        // Get unique ProductIds and BranchIds from StockIns
        var productIdsInStockIn = _context.StockIns
            .Select(si => si.ProductId)
            .Distinct()
            .ToList();

        var branchIdsInStockIn = _context.StockIns
            .Select(si => si.BranchId)
            .Distinct()
            .ToList();

        // Load Products and Branches filtered by stock
        var productsInStock = _context.Products
            .Where(p => productIdsInStockIn.Contains(p.Id))
            .ToList();

        var branchesInStock = _context.Branches
            .Where(b => branchIdsInStockIn.Contains(b.Id))
            .ToList();

        // Load available stock grouped by ProductId & BranchId
        var stockData = _context.StockIns
            .GroupBy(si => new { si.ProductId, si.BranchId })
            .Select(g => new
            {
                ProductId = g.Key.ProductId,
                BranchId = g.Key.BranchId,
                AvailableQty = g.Sum(si => si.Quantity)
            })
            .ToList();

        ViewBag.Products = productsInStock;
        ViewBag.Branches = branchesInStock;
        ViewBag.StockData = stockData;

        return View(model);
    }

    private string GenerateInvoiceNumber()
    {
        return "Hamko-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
    }

    private string GenerateRefNumber()
    {
        return "REF-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Sales sales, List<StockOut> StockOuts)
        {
            if (StockOuts == null || StockOuts.Count == 0)
            {
                TempData["error"] = "No product has been selected!";
                return RedirectToAction("Create");
            }

            var validStockOuts = StockOuts
                .Where(s => s.ProductId != 0 && s.BranchId != 0 && s.Quantity > 0 && s.Price > 0)
                .ToList();

            var duplicateCheck = new HashSet<(int ProductId, int BranchId)>();

            foreach (var stock in validStockOuts)
            {
                var key = (stock.ProductId, stock.BranchId);
                if (duplicateCheck.Contains(key))
                {
                    TempData["error"] = "Multiple entries have been made for the same product and branch!";
                    return RedirectToAction("Create");
                }
                duplicateCheck.Add(key);

                // Fetch stock-in information
                var stockIn = _context.StockIns
                    .Where(s => s.ProductId == stock.ProductId && s.BranchId == stock.BranchId)
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefault();

                // Check if there is sufficient stock
                if (stockIn == null || stockIn.Quantity < stock.Quantity)
                {
                    var product = _context.Products.Find(stock.ProductId);
                    TempData["error"] = $"Insufficient stock for product \"{product?.Name ?? "Unknown"}\" in the selected branch.";
                    return RedirectToAction("Create");
                }

                // If stock is sufficient, subtract the quantity
                stockIn.Quantity -= stock.Quantity;
                _context.Entry(stockIn).Property(s => s.Quantity).IsModified = true;
            }

            // Save the Sales record
            _context.Sales.Add(sales);
            _context.SaveChanges();

            TempData["success"] = "The sale has been completed successfully!";
            return RedirectToAction("Index");
        }









        public IActionResult Details(int id)
        {
            var sales = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.User)
                .FirstOrDefault(s => s.Id == id);

            if (sales == null)
                return NotFound();

            var stockOuts = _context.StockOuts
                .Include(so => so.Product)
                .Include(so => so.Branch)
                .Where(so => so.SalesId == id)
                .ToList();

            var viewModel = new SalesDetailsViewModel
            {
                Sales = sales,
                StockOuts = stockOuts
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var sales = _context.Sales.FirstOrDefault(s => s.Id == id);
            if (sales == null)
            {
                TempData["error"] = "Sale not found!";
                return RedirectToAction("Index");
            }

            // Delete related StockOuts
            var stockOuts = _context.StockOuts.Where(s => s.SalesId == sales.Id).ToList();
            _context.StockOuts.RemoveRange(stockOuts);

            // Delete the Sale
            _context.Sales.Remove(sales);
            _context.SaveChanges();

            TempData["success"] = "Sale and related stock entries deleted successfully!";
            return RedirectToAction("Index");
        }


        // GET: Sales/Edit/5
        public IActionResult Edit(int id)
        {
            var sales = _context.Sales
                .Include(s => s.StockOuts)
                .FirstOrDefault(s => s.Id == id);

            if (sales == null) return NotFound();

            ViewBag.Customers = _context.Customers.ToList();
            ViewBag.Users = _context.User.ToList();
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Branches = _context.Branches.ToList();

            return View(sales);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Sales sales, List<StockOut> StockOuts)
        {
            if (id != sales.Id)
            {
                TempData["error"] = "Invalid Sales ID.";
                return RedirectToAction("Edit", new { id });
            }

            if (StockOuts == null || StockOuts.Count == 0)
            {
                TempData["error"] = "No product has been selected!";
                return RedirectToAction("Edit", new { id });
            }

            var validStockOuts = StockOuts
                .Where(s => s.ProductId != 0 && s.BranchId != 0 && s.Quantity > 0 && s.Price > 0)
                .ToList();

            var duplicateCheck = new HashSet<(int ProductId, int BranchId)>();
            foreach (var stock in validStockOuts)
            {
                var key = (stock.ProductId, stock.BranchId);
                if (duplicateCheck.Contains(key))
                {
                    TempData["error"] = "Multiple entries have been made for the same product and branch!";
                    return RedirectToAction("Edit", new { id });
                }
                duplicateCheck.Add(key);
            }

            // Get existing Sales record from DB
            var existingSales = _context.Sales.Include(s => s.StockOuts).FirstOrDefault(s => s.Id == id);
            if (existingSales == null)
            {
                TempData["error"] = "Sales record not found.";
                return RedirectToAction("Index");
            }

            // Restore stock quantities from old StockOuts
            foreach (var oldStock in existingSales.StockOuts)
            {
                var stockIn = _context.StockIns
                    .Where(si => si.ProductId == oldStock.ProductId && si.BranchId == oldStock.BranchId)
                    .OrderByDescending(si => si.Id)
                    .FirstOrDefault();

                if (stockIn != null)
                {
                    stockIn.Quantity += oldStock.Quantity;
                    _context.Entry(stockIn).Property(si => si.Quantity).IsModified = true;
                }
            }

            // Validate stock sufficiency for new StockOuts
            foreach (var stock in validStockOuts)
            {
                var stockIn = _context.StockIns
                    .Where(si => si.ProductId == stock.ProductId && si.BranchId == stock.BranchId)
                    .OrderByDescending(si => si.Id)
                    .FirstOrDefault();

                if (stockIn == null || stockIn.Quantity < stock.Quantity)
                {
                    var product = _context.Products.Find(stock.ProductId);
                    TempData["error"] = $"Insufficient stock for product \"{product?.Name ?? "Unknown"}\" in the selected branch.";
                    return RedirectToAction("Edit", new { id });
                }
            }

            // Deduct stock quantities for new StockOuts
            foreach (var stock in validStockOuts)
            {
                var stockIn = _context.StockIns
                    .Where(si => si.ProductId == stock.ProductId && si.BranchId == stock.BranchId)
                    .OrderByDescending(si => si.Id)
                    .FirstOrDefault();

                stockIn.Quantity -= stock.Quantity;
                _context.Entry(stockIn).Property(si => si.Quantity).IsModified = true;
            }

            try
            {
                // Update Sales fields (except navigation properties)
                existingSales.Date = sales.Date;
                existingSales.CustomerId = sales.CustomerId;
                existingSales.InvoiceNo = sales.InvoiceNo;
                existingSales.RefNo = sales.RefNo;
                //existingSales.TotalPrice = sales.TotalPrice;
                // ...update any other scalar properties needed

                // Remove old StockOuts
                _context.StockOuts.RemoveRange(existingSales.StockOuts);

                // Add new StockOuts with correct SalesId
                foreach (var stock in validStockOuts)
                {
                    stock.SalesId = id;
                    _context.StockOuts.Add(stock);
                }

                _context.SaveChanges();
                TempData["success"] = "The sale has been updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error updating sale: {ex.Message}";
                return RedirectToAction("Edit", new { id });
            }

            return RedirectToAction("Index");
        }

        // Product search for autocomplete
        [HttpGet]
        public JsonResult SearchProducts(string term)
        {
            var productIdsInStockIn = _context.StockIns
                .Select(si => si.ProductId)
                .Distinct()
                .ToList();

            var products = _context.Products
                .Where(p => productIdsInStockIn.Contains(p.Id) && p.Name.Contains(term))
                .Select(p => new { label = p.Name, value = p.Id })
                .ToList();

            return Json(products);
        }

        // Branch search for autocomplete
        [HttpGet]
        public JsonResult SearchBranches(string term)
        {
            var branchIdsInStockIn = _context.StockIns
                .Select(si => si.BranchId)
                .Distinct()
                .ToList();

            var branches = _context.Branches
                .Where(b => branchIdsInStockIn.Contains(b.Id) && b.BranchName.Contains(term))
                .Select(b => new { label = b.BranchName, value = b.Id })
                .ToList();

            return Json(branches);
        }

        /////////////
        //edit
        /////////////
        [HttpGet]
        public JsonResult SearchCustomers(string term)
        {
            var customerIdsInSales = _context.Sales
                .Select(s => s.CustomerId)
                .Distinct()
                .ToList();

            var customers = _context.Customers
                .Where(c => customerIdsInSales.Contains(c.Id) && c.Name.Contains(term))
                .Select(c => new { label = c.Name, value = c.Id })
                .ToList();

            return Json(customers);
        }

        [HttpGet]
        public JsonResult SearchUsers(string term)
        {
            var userIdsInSales = _context.Sales
                .Select(s => s.UserId)
                .Distinct()
                .ToList();

            var users = _context.User
                .Where(u => userIdsInSales.Contains(u.Id) && u.UserName.Contains(term))
                .Select(u => new { label = u.UserName, value = u.Id })
                .ToList();

            return Json(users);
        }



    }
}
