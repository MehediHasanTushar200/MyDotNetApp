using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;

namespace hamko.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;


        public CategoryController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        //index list
        public IActionResult Index()
        {
            var categories = context.Categories.ToList();
            return View(categories);
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
        public IActionResult Create(CategoryDto categoryDto)
        {
            if (categoryDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }

            // ইমেজ ফাইল নাম তৈরি করা
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(categoryDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(environment.WebRootPath, "categories", newFileName);

            // ইমেজ ফাইল সেভ করা
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                categoryDto.ImageFile.CopyTo(stream);
            }

            // Category অবজেক্ট তৈরি করা
            Category category = new Category()
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Status = categoryDto.Status,
                ImageFileName = newFileName  // এখানে ইমেজ ফাইলের নাম ডাটাবেসে সেভ করা হচ্ছে
            };

            // ডাটাবেসে নতুন Category সেভ করা
            context.Categories.Add(category);
            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        //create store data

        //edit data
        public IActionResult Edit(int id)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return RedirectToAction("Index", "Categories");
            }

            var categoryDto = new CategoryDto()
            {
                Name = category.Name,
                Description = category.Description,
                Status = category.Status,
            };

            ViewData["CategoryId"] = category.Id;
            ViewData["ImageFileName"] = category.ImageFileName;

            return View(categoryDto);
        }
        //edit data
        //Update data

        [HttpPost]
        public IActionResult Edit(int id, CategoryDto categoryDto)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return RedirectToAction("Index", "Categories");
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = category.Id;
                ViewData["ImageFileName"] = category.ImageFileName;

                return View(categoryDto);
            }

            // Update image data
            string newFileName = category.ImageFileName;
            if (categoryDto.ImageFile != null)
            {
                // Generate new file name and save the new image
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(categoryDto.ImageFile.FileName);

                string imageFullPath = Path.Combine(environment.WebRootPath, "categories", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    categoryDto.ImageFile.CopyTo(stream);
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(category.ImageFileName))
                {
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "categories", category.ImageFileName);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }
            }

            // Update category details
            category.ImageFileName = newFileName;
            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;
            category.Status = categoryDto.Status;

            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        //Update data
        //Delete data
        public IActionResult Delete(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return RedirectToAction("Index", "Category");
            }

            // Construct the file path and delete the image if it exists
            string imageFullPath = Path.Combine(environment.WebRootPath, "categories", category.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            // Remove the category record from the database
            context.Categories.Remove(category);
            context.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        //Delete data

        // View data
        public IActionResult Details(int id)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return RedirectToAction("Index", "Category");
            }

            return View(category);
        }
        // View data



    }
}
