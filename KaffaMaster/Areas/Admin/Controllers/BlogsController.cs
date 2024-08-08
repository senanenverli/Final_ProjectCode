﻿using KaffaMaster.Contexts;
using KaffaMaster.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaffaMaster.Areas.Admin.ViewModel;
using KaffaMaster.Helpers.Extencions;

namespace KaffaMaster.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class BlogsController : Controller
{
    private readonly KaffaDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public BlogsController(KaffaDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var blog = _context.Blogs.ToList();

        return View(blog);
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
    public async Task<IActionResult> Create(BlogViewModel blog)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        if (blog.PosterImage.CheckFileSize(3000))
        {
            ModelState.AddModelError("Image", "Too Big!");
            return View();
        }
        if (!blog.PosterImage.CheckFileType("image/"))
        {
            ModelState.AddModelError("Image", "sekil olsun");
            return View();
        }
        string fileName = $"{Guid.NewGuid()}-{blog.PosterImage.FileName}";
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "blog", fileName);
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await blog.PosterImage.CopyToAsync(stream);
        }
        Blog newblog = new()
        {
            Title = blog.Title,
            Description = blog.Description,
            Author = blog.Author,
            Content = blog.Content,
            FamousWord = blog.FamousWord,
            AuthorComment = blog.AuthorComment,
            PosterImage = fileName,
            CreatedDate = DateTime.UtcNow,
			UpdateDate = DateTime.UtcNow,
            isDeleted  = blog.isDeleted ,

            Image = "Default.png"
        };
        await _context.Blogs.AddAsync(newblog);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted); ;
        if (blog == null)
        {
            return NotFound();
        }
        return View(blog);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deleteblog(int id)
    {

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted); ;
        if (blog == null)
        {
            return NotFound();
        }
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "blog", blog.PosterImage);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        blog.isDeleted = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detail(int id)
    {

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted); ;
        if (blog == null)
        {
            return NotFound();
        }
        return View(blog);
    }
    public async Task<IActionResult> Update(int id)
    {
        var blog = await _context.Blogs.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted);
        if (blog == null)
            return NotFound();

        BlogUpdateViewModel model = new()
        {
            Title = blog.Title,
            Description = blog.Description,
            Author = blog.Author,
            Content = blog.Content,
            FamousWord = blog.FamousWord,
            AuthorComment = blog.AuthorComment,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, BlogUpdateViewModel blog)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var updateblog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted);
        if (updateblog == null)
        {
            return NotFound();
        }

        if (blog.PosterImage != null)
        {
            if (blog.PosterImage.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "sekilin olcusu hedden artiqdir");
                return View();
            }

            if (!blog.PosterImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "sadece sekil olmalidir");
                return View();
            }

            string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "blog");
            string path = Path.Combine(basePath, updateblog.PosterImage);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            string fileName = $"{Guid.NewGuid()}-{blog.PosterImage.FileName}";
            path = Path.Combine(basePath, fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await blog.PosterImage.CopyToAsync(stream);
            }
            updateblog.PosterImage = fileName;
        }


        updateblog.Title = blog.Title;
        updateblog.Description = blog.Description;
        updateblog.Author = blog.Author;
        updateblog.Content = blog.Content;
        updateblog.FamousWord = blog.FamousWord;
        updateblog.AuthorComment = blog.AuthorComment;
        updateblog.CreatedDate = DateTime.UtcNow;
        updateblog.UpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
