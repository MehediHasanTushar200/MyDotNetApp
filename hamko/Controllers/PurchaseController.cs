﻿using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hamko.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PurchaseController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            var purchase = _context.Purchases.ToList();
            return View(purchase);
        }

        public IActionResult Create()
        {

            var model = new Purchase
            {
                Date = DateTime.Now,
                InvoiceNo = GenerateInvoiceNumber(), // Optional if needed
                RefNo = GenerateRefNumber(),         // Optional if needed
                StockIns = new List<StockIn> { new StockIn() }
            };

            ViewBag.Products = _context.Products.ToList();
            ViewBag.Branches = _context.Branches.ToList();

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
        public IActionResult Create(Purchase purchase, List<StockIn> StockIns)
        {


            // Save Purchase first
            _context.Purchases.Add(purchase);
            _context.SaveChanges(); // generates purchase.Id

            // Save valid StockIn rows only
            foreach (var stock in StockIns)
            {
                if (stock.ProductId == 0 || stock.BranchId == 0 || stock.Quantity <= 0 || stock.Price <= 0)
                    continue;

                stock.PurchaseId = purchase.Id; // Assign FK
                _context.StockIns.Add(stock);
            }

            //_context.SaveChanges(); 

            TempData["success"] = "Purchase has been saved successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var purchase = _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == id);
 

            if (purchase == null)
                return NotFound();

            var stockIns = _context.StockIns
                .Include(s => s.Product)
                .Include(s => s.Branch)
                .Where(s => s.PurchaseId == id)
                .ToList();

            var viewModel = new PurchaseDetailsViewModel
            {
                Purchase = purchase,
                StockIns = stockIns
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var purchase = _context.Purchases.FirstOrDefault(p => p.Id == id);
            if (purchase != null)
            {
                // Remove related StockIns first
                var stockIns = _context.StockIns.Where(s => s.PurchaseId == id).ToList();
                _context.StockIns.RemoveRange(stockIns);

                _context.Purchases.Remove(purchase);
                _context.SaveChanges();

                TempData["success"] = "The record has been deleted.";
            }

            return RedirectToAction("Index");
        }

        // GET: Purchase/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.StockIns)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            // Dropdown lists
            ViewBag.Suppliers = await _context.Supplier.ToListAsync();
            ViewBag.Users = await _context.User.ToListAsync();
            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Branches = await _context.Branches.ToListAsync();

            return View(purchase);
        }

        // POST: Purchase/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Purchase purchase)
        {
            if (id != purchase.Id)
                return NotFound();

            // Remove old StockIns
            var existingStockIns = _context.StockIns.Where(s => s.PurchaseId == id);
            _context.StockIns.RemoveRange(existingStockIns);
            await _context.SaveChangesAsync();

            // Re-add updated StockIns
            if (purchase.StockIns != null && purchase.StockIns.Any())
            {
                foreach (var stock in purchase.StockIns)
                {
                    stock.PurchaseId = purchase.Id;
                    _context.StockIns.Add(stock);
                }
            }

            // Detach tracking on original Purchase to avoid duplicate key issues
            var existingPurchase = await _context.Purchases.FindAsync(id);
            if (existingPurchase != null)
            {
                _context.Entry(existingPurchase).State = EntityState.Detached;
            }

            // Update Purchase
            _context.Purchases.Update(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Example Controller actions for autocomplete

        [HttpGet]
        public JsonResult SearchProducts(string term)
        {
            var products = _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => new { label = p.Name, value = p.Id })
                .ToList();
            return Json(products);
        }

        [HttpGet]
        public JsonResult SearchBranches(string term)
        {
            var branches = _context.Branches
                .Where(b => b.BranchName.Contains(term))
                .Select(b => new { label = b.BranchName, value = b.Id })
                .ToList();
            return Json(branches);
        }


    }

}
