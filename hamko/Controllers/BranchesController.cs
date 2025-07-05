using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;

namespace hamko.Controllers
{
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public BranchesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var branches = context.Branches.ToList();
            return View(branches);
        }

        //create data form
        public IActionResult Create()
        {
            return View();
        }
        //create data form

        //create store data

        [HttpPost]
        public IActionResult Create(BranchDto branchDto)
        {
            if (branchDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(branchDto);
            }

            // ইমেজ ফাইল নাম তৈরি করা
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(branchDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(environment.WebRootPath, "branches", newFileName);

            // ইমেজ ফাইল সেভ করা
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                branchDto.ImageFile.CopyTo(stream);
            }

            // Branch অবজেক্ট তৈরি করা
            Branch branch = new Branch()
            {
                BranchName = branchDto.BranchName,
                Description = branchDto.Description,
                Status = branchDto.Status,
                ImageFileName = newFileName  // এখানে ইমেজ ফাইলের নাম ডাটাবেসে সেভ করা হচ্ছে

            };

            // ডাটাবেসে নতুন Branch সেভ করা
            context.Branches.Add(branch);
            context.SaveChanges();

            return RedirectToAction("Index", "Branches");
        }

        //create store data

        public IActionResult Edit(int id)
        {
            var branch = context.Branches.Find(id);

            if (branch == null)
            {
                return RedirectToAction("Index", "Branches");
            }

            var branchDto = new BranchDto()
            {
                BranchName = branch.BranchName,
                Description = branch.Description,
                Status = branch.Status,


            };

            ViewData["BranchId"] = branch.Id;
            ViewData["ImageFileName"] = branch.ImageFileName;

            return View(branchDto);
        }
        //edit data

        //Update data

        [HttpPost]
        public IActionResult Edit(int id, BranchDto branchDto)
        {
            var branch = context.Branches.Find(id);

            if (branch == null)
            {
                return RedirectToAction("Index", "Branches");
            }

            if (!ModelState.IsValid)
            {
                ViewData["BranchId"] = branch.Id;
                ViewData["ImageFileName"] = branch.ImageFileName;

                return View(branchDto);
            }

            // Update image data
            string newFileName = branch.ImageFileName;
            if (branchDto.ImageFile != null)
            {
                // Generate new file name and save the new image
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(branchDto.ImageFile.FileName);

                string imageFullPath = Path.Combine(environment.WebRootPath, "branches", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    branchDto.ImageFile.CopyTo(stream);
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(branch.ImageFileName))
                {
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "branches", branch.ImageFileName);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }
            }

            // Update student details
            branch.ImageFileName = newFileName;
            branch.BranchName = branchDto.BranchName;
            branch.Description = branchDto.Description;
            branch.Status = branchDto.Status;


            context.SaveChanges();

            return RedirectToAction("Index", "Branches");
        }

        //Update data

        //Delete data
        public IActionResult Delete(int id)
        {
            var branch = context.Branches.Find(id);
            if (branch == null)
            {
                return RedirectToAction("Index", "Branches");
            }

            // Construct the file path and delete the image if it exists
            string imageFullPath = Path.Combine(environment.WebRootPath, "Branches", branch.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            // Remove the student record from the database
            context.Branches.Remove(branch);
            context.SaveChanges();

            return RedirectToAction("Index", "Branches");
        }

        //Delete data

        // View data
        public IActionResult Details(int id)
        {
            var branch = context.Branches.Find(id);

            if (branch == null)
            {
                return RedirectToAction("Index", "Branches");
            }

            return View(branch);
        }
        // View data

    }
}
