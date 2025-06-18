using hamko.Models;
using hamko.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hamko.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBatteryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiBatteryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ApiBattery
        [HttpGet]
        public async Task<IActionResult> GetBatteries()
        {
            var batteries = await _context.Batteries.ToListAsync();
            return Ok(batteries); // Return the list of batteries as JSON
        }
    }
}

