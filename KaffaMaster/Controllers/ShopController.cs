using KaffaMaster.Contexts;
using KaffaMaster.Models;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KaffaMaster.Controllers;

public class ShopController : Controller
{
    private readonly KaffaDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public ShopController(KaffaDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? input, int? categoryId)
    {
        ViewData["Categories"] = await _context.Categories.Where(n => !n.isDeleted).ToListAsync();
        if (categoryId != null && input == null)
        {
            var filteredProducts = await _context.Products.Where(p => p.CategoryId == categoryId).Where(p => !p.isDeleted).Include(p => p.Category).ToListAsync();
            return View(filteredProducts);
        }else if (input != null && categoryId == null)
        {
            var filteredProducts = await _context.Products.Where(p => p.Title.ToLower().Contains(input.ToLower())).Where(p => !p.isDeleted).Include(p => p.Category).ToListAsync();
            return View(filteredProducts);
        }

        var products = await _context.Products.Where(p => p.isStocked).Where(p => !p.isDeleted).Take(8).Include(p => p.Category).ToListAsync();
        return View(products);
    }
    [Authorize]
    public async Task<IActionResult> AddProductToBasket(int productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && !p.isDeleted);
        if (product == null)
        {
            return NotFound();
        }
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        var basketItem = await _context.BasketItems.Include(n => n.Products).FirstOrDefaultAsync(b => b.AppUserId == user.Id);
        if (basketItem == null)
        {
             basketItem = new BasketItem()
            {
                AppUserId = user.Id,
                Count = 0,
                CreatedDate = DateTime.UtcNow,
                Products = new List<Product>()
            };

            await _context.BasketItems.AddAsync(basketItem);

            //basketItem.Products.Add(product);
        }
        else
        {
            basketItem.Products.Add(product);
            basketItem.Count++;
            _context.BasketItems.Update(basketItem);
        }
        //_context.BasketItems.Update(basketItem);
        await _context.SaveChangesAsync();


        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> LoadMore(int skip)
    {
        int productCount = await _context.Products.Where(p => !p.isDeleted).CountAsync();
        if (skip >= productCount)
            return BadRequest();


        List<Product> products = await _context.Products.Where(p => !p.isDeleted)
            .Skip(skip).Take(8).ToListAsync();

        return View(nameof(Index));
    }


}

