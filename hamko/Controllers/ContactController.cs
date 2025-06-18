using hamko.Service;
using Microsoft.AspNetCore.Mvc;

namespace hamko.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ContactController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        // Index - Contact List
        public IActionResult Index()
        {
            var contacts = context.Contacts.ToList();
            return View(contacts);
        }

        // Delete Contact
        public IActionResult Delete(int id)
        {
            var contact = context.Contacts.Find(id);
            if (contact == null)
            {
                return RedirectToAction("Index", "Contact");
            }

            // Remove the contact record from the database
            context.Contacts.Remove(contact);
            context.SaveChanges();

            return RedirectToAction("Index", "Contact");
        }
    }
}
