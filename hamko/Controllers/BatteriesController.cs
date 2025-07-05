using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace hamko.Controllers
{
    public class BatteriesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        // কনস্ট্রাক্টর
        public BatteriesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        // ব্যাটারির লিস্ট দেখানোর জন্য Index Method
        public IActionResult Index()
        {
            var batteries = context.Batteries.ToList();
            return View(batteries);
        }

        // নতুন ব্যাটারি যোগ করার ফর্ম দেখানোর জন্য Create Method
        public IActionResult Create()
        {
            return View();
        }

        //create store data

        [HttpPost]
        public IActionResult Create(BatteryDto batteryDto)
        {
            if (batteryDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(batteryDto);
            }

            // ইমেজ ফাইল নাম তৈরি করা
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(batteryDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(environment.WebRootPath, "batteries", newFileName);

            // ইমেজ ফাইল সেভ করা
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                batteryDto.ImageFile.CopyTo(stream);
            }

            // Battery অবজেক্ট তৈরি করা
            Battery battery = new Battery()
            {
                Name = batteryDto.Name,
                Type = batteryDto.Type,
                Description = batteryDto.Description,
                Status = batteryDto.Status,
                ImageFileName = newFileName  // এখানে ইমেজ ফাইলের নাম ডাটাবেসে সেভ করা হচ্ছে

            };

            // ডাটাবেসে নতুন Branch সেভ করা
            context.Batteries.Add(battery);
            context.SaveChanges();

            return RedirectToAction("Index", "Batteries");
        }

        //create store data

        public IActionResult Edit(int id)
        {
            var battery = context.Batteries.Find(id);

            if (battery == null)
            {
                return RedirectToAction("Index", "Batteries");
            }

            var batteryDto = new BatteryDto()
            {
                Name = battery.Name,
                Type = battery.Type,
                Description = battery.Description,
                Status = battery.Status,


            };

            ViewData["BatteryId"] = battery.Id;
            ViewData["ImageFileName"] = battery.ImageFileName;

            return View(batteryDto);
        }
        //edit data

        //Update data

        [HttpPost]
        public IActionResult Edit(int id, BatteryDto batteryDto)
        {
            var battery = context.Batteries.Find(id);

            if (battery == null)
            {
                return RedirectToAction("Index", "Batteries");
            }

            if (!ModelState.IsValid)
            {
                ViewData["BatteryId"] = battery.Id;
                ViewData["ImageFileName"] = battery.ImageFileName;

                return View(batteryDto);
            }

            // Update image data
            string newFileName = battery.ImageFileName;
            if (batteryDto.ImageFile != null)
            {
                // Generate new file name and save the new image
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(batteryDto.ImageFile.FileName);

                string imageFullPath = Path.Combine(environment.WebRootPath, "batteries", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    batteryDto.ImageFile.CopyTo(stream);
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(battery.ImageFileName))
                {
                    string oldImageFullPath = Path.Combine(environment.WebRootPath, "batteries", battery.ImageFileName);
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        System.IO.File.Delete(oldImageFullPath);
                    }
                }
            }

            // Update student details
            battery.ImageFileName = newFileName;
            battery.Name = batteryDto.Name;
            battery.Type = batteryDto.Type;
            battery.Description = batteryDto.Description;
            battery.Status = batteryDto.Status;


            context.SaveChanges();

            return RedirectToAction("Index", "Batteries");
        }

        //Update data

        //Delete data
        public IActionResult Delete(int id)
        {
            var battery = context.Batteries.Find(id);
            if (battery == null)
            {
                return RedirectToAction("Index", "Batteries");
            }

            // Construct the file path and delete the image if it exists
            string imageFullPath = Path.Combine(environment.WebRootPath, "Batteries", battery.ImageFileName);
            if (System.IO.File.Exists(imageFullPath))
            {
                System.IO.File.Delete(imageFullPath);
            }

            // Remove the student record from the database
            context.Batteries.Remove(battery);
            context.SaveChanges();

            return RedirectToAction("Index", "Batteries");
        }

        //Delete data

        // View data
        public IActionResult Details(int id)
        {
            var battery = context.Batteries.Find(id);

            if (battery == null)
            {
                return RedirectToAction("Index", "Batteries");
            }

            return View(battery);
        }
        // View data

    }

}
