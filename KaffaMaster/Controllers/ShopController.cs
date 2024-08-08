using KaffaMaster.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MimeKit.Tnef;
using KaffaMaster.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using KaffaMaster.ViewModel;
using MailKit.Search;

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

    public async Task<IActionResult> Index(string? input,int? categoryId)
    {
        int productCount = await _context.Products.Where(p => !p.isDeleted).CountAsync();
        ViewBag.ProductCount = productCount;
        IQueryable<Product> query = _context.Products.AsQueryable(); 
        ViewData["Categories"] = await _context.Categories.ToListAsync();
        if(categoryId!=null){
            
            query = query.Where(p=> p.CategoryId==categoryId);
        
        }
        if(input != null)
        {
                var searchTerm = input.ToLower();
                var filteredProducts = await _context.Products.Where(p => p.Title.ToLower().Contains(searchTerm)).Where(p => !p.isDeleted).Include(p => p.Category).ToListAsync();
                return View(filteredProducts);
        }
        var products = await _context.Products.Where(p => p.isStocked).Where(p => !p.isDeleted).Include(p => p.Category).ToListAsync();
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

        var basketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.ProductId == productId && b.AppUserId == user.Id);
        if (basketItem == null)
        {

            BasketItem newasketItem = new BasketItem()
            {
                ProductId = productId,
                AppUserId = user.Id,
                Count = 1,
                CreatedDate = DateTime.UtcNow,
            };

            await _context.BasketItems.AddAsync(newasketItem);
        }
        else
        {
            basketItem.Count++;
        }
        await _context.SaveChangesAsync();


        return RedirectToAction(nameof(Index));
    }

    //[HttpGet]
    //public async Task<IActionResult> Search(string input)
    //{
    //    if (input != null)
    //    {
    //        var searchTerm = input.ToLower();
    //        var filteredProducts = await _context.Products.Where(p => p.Title.ToLower().Contains(searchTerm)).Where(p => !p.isDeleted).Include(p => p.Category).ToListAsync();
    //        return View(filteredProducts);
    //    }
    //    else
    //    {
    //        return View(null);
    //    }
    //}
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

