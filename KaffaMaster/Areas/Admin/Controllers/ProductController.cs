using Helpers;
using KaffaMaster.Areas.Admin.ViewModel;
using KaffaMaster.Contexts;
using KaffaMaster.Helpers.Extencions;
using KaffaMaster.Models;
using KaffaMaster.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KaffaMaster.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class ProductController : Controller
{
    private readonly KaffaDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public ProductController(KaffaDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        var products = _context.Products.ToList();


        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.Where(n => !n.isDeleted).ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();

        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel products)
    {
        ViewBag.Categories = await _context.Categories.Where(n => !n.isDeleted).ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        if (!ModelState.IsValid)
        {
            return View();
        }
        if (products.PosterImage.CheckFileSize(3000))
        {
            ModelState.AddModelError("PosterImage", "Image size is too big");
            return View();
        }
        if (!products.PosterImage.CheckFileType("image/"))
        {
            ModelState.AddModelError("PosterImage", "Only images are allowed");
            return View();
        }
        string fileName = $"{Guid.NewGuid()}-{products.PosterImage.FileName}";
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", fileName);
        FileStream stream = new FileStream(path, FileMode.Create);
        await products.PosterImage.CopyToAsync(stream);
        stream.Dispose();
        Product newproduct = new()
        {
            Title = products.Title,
            Price = products.Price,
            BrandId = products.BrandId,
            CategoryId = products.CategoryId,
            PosterImage = fileName,
            isStocked = true,
            isDeleted = false,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };

        await _context.Products.AddAsync(newproduct);
        await _context.SaveChangesAsync();

        bool isSucced = await SendEmailToSubscribers(newproduct);

        if (!isSucced)
        {
            ModelState.AddModelError("", "Ismaric gonderilmedi");
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<bool> SendEmailToSubscribers(Product product)
    {
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

        EmailHelper emailHelper = new EmailHelper(_configuration);
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

        return true;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var products = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && !p.isDeleted);
        if (products == null)
        {
            return NotFound();
        }
        return View(products);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var products = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.isDeleted);
        if (products == null)
        {
            return NotFound();
        }

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", products.PosterImage);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        products.isDeleted = true;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detail(int id)
    {
        var products = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && !p.isDeleted);
        if (products == null)
        {
            return NotFound();
        }
        return View(products);

    }
    public async Task<IActionResult> Update(int id)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        var products = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.isDeleted);
        if (products == null)
        {
            return NotFound();
        }
        ProductUpdateViewModel model = new()
        {
            Title = products.Title,
            Price = products.Price,
            Rating = products.Rating,
            BrandId = products.BrandId,
            CategoryId = products.CategoryId,
            isStocked = true,
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ProductUpdateViewModel products)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();
        var updateProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.isDeleted);
        if (products == null)
        {
            return NotFound();
        }

        if (products.PosterImage != null)
        {
            if (products.PosterImage.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "Image size is too big");
                return View();
            }

            if (!products.PosterImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "Only images are allowed");
                return View();
            }

            string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images");
            string path = Path.Combine(basePath, updateProduct.PosterImage);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            string fileName = $"{Guid.NewGuid()}-{products.PosterImage.FileName}";
            path = Path.Combine(basePath, fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await products.PosterImage.CopyToAsync(stream);
            }
            updateProduct.PosterImage = fileName;
        }

        updateProduct.Title = products.Title;
        updateProduct.Price = products.Price;
        updateProduct.Rating = products.Rating;
        updateProduct.BrandId = products.BrandId;
        updateProduct.CategoryId = products.CategoryId;
        updateProduct.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
