using Helpers;
using KaffaMaster.Contexts;
using KaffaMaster.Models;
using KaffaMaster.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace KaffaMaster.Controllers;

public class ProductController : Controller
{
    private readonly KaffaDbContext _context;
    private readonly EmailHelper emailHelper;
    private readonly UserManager<AppUser> _userManager;
    public ProductController(KaffaDbContext context, EmailHelper emailHelper, UserManager<AppUser> userManager)
    {
        _context = context;
        this.emailHelper = emailHelper;
        _userManager = userManager;
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Price")] Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();

            
            var subscriptions = await _userManager.Users.Where(n => n.IsSubscribed == true).ToListAsync();
            var subject = "New Product Added";
            var productLink = Url.Action("Details", "Products", new { id = product.Id }, Request.Scheme);

            
            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>New Product Added</title>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; }}
                    .container {{ max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }}
                    h1 {{ color: #333; }}
                    p {{ font-size: 16px; line-height: 1.5; }}
                    a {{ color: #007bff; text-decoration: none; }}
                    a:hover {{ text-decoration: underline; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>New Product Alert!</h1>
                    <p>We are excited to announce a new product in our store:</p>
                    <p><strong>Product Name:</strong> {product.Title}</p>
                    <p><strong>Price:</strong> {product.Price:C}</p>
                    <p>Click <a href='{productLink}' target='_blank'>here</a> to view the product and make a purchase.</p>
                    <p>Thank you for being a valued subscriber!</p>
                    <p>Best regards,<br>Your Company</p>
                </div>
            </body>
            </html>";

            
            foreach (var subscription in subscriptions)
            {
                var mailRequest = new MailRequest
                {
                    ToEmail = subscription.Email,
                    Subject = subject,
                    Body = body
                };

                await emailHelper.SendEmailAsync(mailRequest);
            }

            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }
    public async Task<IActionResult> ProductDetail(int id)
    {
        var products = await _context.Products.Where(p => p.isStocked).Include(p => p.Category).Include(p => p.Brand).FirstOrDefaultAsync(p => p.Id == id);
		if (products == null)
        {
            return NotFound();
        }

        ProductDetailViewModel model = new()
        {
            Title = products.Title,
            Price = products.Price,
            CategoryName = products.Category.CategoryName,
            BrandName = products.Brand.BrandName,
            Rating = products.Rating,
            //Description = products.Description,
            
        };
        await _context.SaveChangesAsync();
        return View(model);
    }
}
