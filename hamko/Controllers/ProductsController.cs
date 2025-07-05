using System;
using System.IO;
using System.Linq;
using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace hamko.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Products
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToList();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Brands = _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToList();

            ViewBag.Categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = _context.Brands
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name
                    })
                    .ToList();

                ViewBag.Categories = _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();

                return View(productDto);
            }

            // Save Image
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                 Path.GetExtension(productDto.ImageFile?.FileName);
            string imageFullPath = Path.Combine(_environment.WebRootPath, "products", newFileName);

            if (productDto.ImageFile != null)
            {
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
            }

            // Create Product
            Product product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Status = productDto.Status,
                BrandId = productDto.BrandId,
                CategoryId = productDto.CategoryId,
                ImageFileName = newFileName
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    var product = _context.Products.Find(id);

        //    if (product == null)
        //    {
        //        return RedirectToAction("Index", "Products");
        //    }

        //    var productDto = new ProductDto()
        //    {
        //        Name = product.Name,
        //        Description = product.Description,
        //        Status = product.Status,
        //        BrandId = product.BrandId,
        //        CategoryId = product.CategoryId,
        //        ImageFileName = product.ImageFileName
        //    };

        //    // Adding Brand and Category data to ViewBag for dropdowns
        //    ViewBag.Brands = _context.Brands
        //        .Select(b => new SelectListItem
        //        {
        //            Value = b.Id.ToString(),
        //            Text = b.Name
        //        })
        //        .ToList();

        //    ViewBag.Categories = _context.Categories
        //        .Select(c => new SelectListItem
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.Name
        //        })
        //        .ToList();

        //    return View(productDto);
        //}

        //[HttpPost]
        //public IActionResult Edit(int id, ProductDto productDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Brands = _context.Brands
        //            .Select(b => new SelectListItem
        //            {
        //                Value = b.Id.ToString(),
        //                Text = b.Name
        //            })
        //            .ToList();

        //        ViewBag.Categories = _context.Categories
        //            .Select(c => new SelectListItem
        //            {
        //                Value = c.Id.ToString(),
        //                Text = c.Name
        //            })
        //            .ToList();

        //        return View(productDto);
        //    }

        //    var product = _context.Products.Find(id);
        //    if (product == null)
        //    {
        //        return RedirectToAction("Index", "Products");
        //    }

        //    // Update product details
        //    product.Name = productDto.Name;
        //    product.Description = productDto.Description;
        //    product.Status = productDto.Status;
        //    product.BrandId = productDto.BrandId;
        //    product.CategoryId = productDto.CategoryId;

        //    // Check if an image was uploaded
        //    if (productDto.ImageFile != null)
        //    {
        //        // Delete old image if exists
        //        var oldImagePath = Path.Combine(_environment.WebRootPath, "products", product.ImageFileName);
        //        if (System.IO.File.Exists(oldImagePath))
        //        {
        //            System.IO.File.Delete(oldImagePath);
        //        }

        //        // Save new image
        //        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
        //                             Path.GetExtension(productDto.ImageFile.FileName);
        //        string imageFullPath = Path.Combine(_environment.WebRootPath, "products", newFileName);

        //        using (var stream = System.IO.File.Create(imageFullPath))
        //        {
        //            productDto.ImageFile.CopyTo(stream);
        //        }

        //        product.ImageFileName = newFileName;
        //    }

        //    // Save changes to the database
        //    _context.SaveChanges();

        //    return RedirectToAction("Index");
        //}



    }
}
