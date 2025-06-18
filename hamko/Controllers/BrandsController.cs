
using System;
using System.Net;
using System.Numerics;
using System.Reflection;
using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;

namespace hamko.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment; 


        public BrandsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        //index list
        public IActionResult Index()
        {
            var brands = context.Brands.ToList();
            return View(brands);
        }
        //index list

        //create data form
        public IActionResult Create()
        {
            return View();
        }
        //create data form

        //create store data

        [HttpPost]
        public IActionResult Create(BrandDto brandDto)
        {
            if (brandDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(brandDto);
            }

            // ইমেজ ফাইল নাম তৈরি করা
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(brandDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(environment.WebRootPath, "brands", newFileName);

            // ইমেজ ফাইল সেভ করা
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                brandDto.ImageFile.CopyTo(stream);
            }

            // Brand অবজেক্ট তৈরি করা
            Brand brand = new Brand()
            {
                Name = brandDto.Name,
                Description = brandDto.Description,
                Status = brandDto.Status,
                ImageFileName = newFileName  // এখানে ইমেজ ফাইলের নাম ডাটাবেসে সেভ করা হচ্ছে
            };

            // ডাটাবেসে নতুন Brand সেভ করা
            context.Brands.Add(brand);
            context.SaveChanges();

            return RedirectToAction("Index", "Brands");
        }

        //create store data

        //edit data
        public IActionResult Edit (int id)
        {
            var brand = context.Brands.Find(id);

            if (brand == null)
            {
                return RedirectToAction("Index", "Brands");
            }

            var brandDto = new BrandDto()
            {
                Name = brand.Name,
                Description = brand.Description,
                Status = brand.Status,
            };

            ViewData["BrandId"] = brand.Id;
            ViewData["ImageFileName"] = brand.ImageFileName;

            return View(brandDto);
        }
        //edit data

        //Update data

        [HttpPost]
        public IActionResult Edit(int id, BrandDto brandDto)
        {
            var brand = context.Brands.Find(id);

            if (brand == null)
            {
                return RedirectToAction("Index", "Brands");
            }

            if (!ModelState.IsValid)
            {
                ViewData["StudentId"] = brand.Id;
                ViewData["ImageFileName"] = brand.ImageFileName;

                return View(brandDto);
            }

            // Update image data
            string newFileName = brand.ImageFileName;
            if (brandDto.ImageFile != null)
            {
                // Generate new file name and save the new image
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(brandDto.ImageFile.FileName);

                string imageFullPath = Path.Combine(environment.WebRootPath, "brands", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    brandDto.ImageFile.CopyTo(stream);
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(brand.ImageFileName))
                {
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "brands", brand.ImageFileName);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }
            }

            // Update brand details
            brand.ImageFileName = newFileName;
            brand.Name = brandDto.Name;
            brand.Description = brandDto.Description;
            brand.Status = brandDto.Status;

            context.SaveChanges();

            return RedirectToAction("Index", "Brands");
        }

        //Update data

        //Delete data
        public IActionResult Delete(int id)
        {
            var brand = context.Brands.Find(id);
            if (brand == null)
            {
                return RedirectToAction("Index", "Brands");
            }

            // Construct the file path and delete the image if it exists
            string imageFullPath = Path.Combine(environment.WebRootPath, "brands", brand.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            // Remove the brand record from the database
            context.Brands.Remove(brand);
            context.SaveChanges();

            return RedirectToAction("Index", "Brands");
        }

        //Delete data

        // View data
        public IActionResult Details(int id)
        {
            var brand = context.Brands.Find(id);

            if (brand == null)
            {
                return RedirectToAction("Index", "Brands");
            }

            return View(brand);
        }
        // View data

    }
}
