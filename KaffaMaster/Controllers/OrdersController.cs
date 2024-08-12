
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using KaffaMaster.Models;
using KaffaMaster.Contexts;

namespace KaffaMaster.Controllers;

public class OrdersController : Controller
{
    private readonly KaffaDbContext _context;

    public OrdersController(KaffaDbContext context)
    {
        _context = context;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        BasketItem basketItem = await _context.BasketItems.Include(n => n.Products).FirstOrDefaultAsync();
        return View(basketItem);
    }

   
    
    public async Task<IActionResult> Delete(int id)
    {

        var basketItem = await _context.BasketItems.Include(n => n.AppUser).Where(n => n.AppUser.UserName == User.Identity.Name).Include(b => b.Products)
            .FirstOrDefaultAsync();
        if (basketItem == null)
        {
            return NotFound();
        }

        Product product = await _context.Products.Where(n => n.Id == id).FirstOrDefaultAsync();

        if(product == null)
        {
            return NotFound();
        }

        basketItem.Products.Remove(product);
        _context.BasketItems.Update(basketItem);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: Orders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var basketItem = await _context.BasketItems.FindAsync(id);
        _context.BasketItems.Remove(basketItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
