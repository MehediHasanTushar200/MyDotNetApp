
using Microsoft.AspNetCore.Mvc;
using hamko.Service;
using hamko.Models;

namespace hamko.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiContactController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit")]
        public IActionResult SubmitContact([FromBody] Contact model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            _context.Contacts.Add(model);
            _context.SaveChanges();
            return Ok(new { message = "Contact saved successfully!" });
        }
    }
}
