using KaffaMaster.Contexts;
using KaffaMaster.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaffaMaster.Controllers;

public class ContactController : Controller
{
    private readonly KaffaDbContext _context;

    public ContactController(KaffaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(Comment comment)
    {
        if (!ModelState.IsValid)
            return View();

        Comment comments = new()
        {
            Name = comment.Name,
            Comments = comment.Comments,
            Email = comment.Email,
        };
        await _context.Comments.AddAsync(comments);
        await _context.SaveChangesAsync();

        if (ModelState.IsValid)
        {
            TempData["Success"] = "Thanks for your attention  ";
            return RedirectToAction(nameof(Index));
        }
        return View();

    }
}
