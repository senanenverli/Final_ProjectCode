using KaffaMaster.Areas.Admin.ViewModel;
using KaffaMaster.Contexts;
using KaffaMaster.Helpers.Extencions;
using KaffaMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KaffaMaster.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class CategoryController : Controller
{
    private readonly KaffaDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public CategoryController(KaffaDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var category = _context.Categories.ToList();

        return View(category);
    }
    public async Task<IActionResult> Create()
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel category)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        if (category.Image.CheckFileSize(3000))
        {
            ModelState.AddModelError("Image", "kokelifsen");
            return View();
        }
        if (!category.Image.CheckFileType("image/"))
        {
            ModelState.AddModelError("Image", "alinmirsa diresme");
            return View();
        }
        string fileName = $"{Guid.NewGuid()}-{category.Image.FileName}";
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", fileName);
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await category.Image.CopyToAsync(stream);
        }
        Category newcategory = new()
        {
            CategoryName = category.CategoryName,
            Image = fileName,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };
        await _context.Categories.AddAsync(newcategory);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.isDeleted);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.isDeleted);
        if (category == null)
        {
            return NotFound();
        }
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", category.Image);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        category.isDeleted = true;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detail(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.isDeleted);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }
    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && !c.isDeleted);
        if (category == null)
            return NotFound();

        CategoryUpdateViewModel model = new()
        {
            CategoryName = category.CategoryName,
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, CategoryUpdateViewModel category)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var updateCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.isDeleted);
        if (updateCategory == null)
        {
            return NotFound();
        }

        if (category.Image != null)
        {
            if (category.Image.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "sekil olcusu hedden artiqdir");
                return View();
            }

            if (!category.Image.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "sadece sekil olmalidir");
                return View();
            }

            string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "categories");
            string path = Path.Combine(basePath, updateCategory.Image);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            string fileName = $"{Guid.NewGuid()}-{category.Image.FileName}";
            path = Path.Combine(basePath, fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await category.Image.CopyToAsync(stream);
            }
            updateCategory.Image = fileName;
        }


        updateCategory.CategoryName = category.CategoryName;
        updateCategory.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
