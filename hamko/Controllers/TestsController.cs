using hamko.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hamko.Service;

public class TestsController : Controller
{
   


    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public TestsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }


    public async Task<IActionResult> Index()
    {
        return View(await _context.Tests.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Test test)
    {
        if (ModelState.IsValid)
        {
            _context.Add(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(test);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var test = await _context.Tests.FindAsync(id);
        if (test == null) return NotFound();

        return View(test);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Test test)
    {
        if (id != test.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(test);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tests.Any(e => e.Id == test.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(test);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var test = await _context.Tests.FirstOrDefaultAsync(m => m.Id == id);
        if (test == null) return NotFound();

        return View(test);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var test = await _context.Tests.FindAsync(id);
        if (test != null)
        {
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
