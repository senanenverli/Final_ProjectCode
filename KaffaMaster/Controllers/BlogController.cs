using KaffaMaster.Contexts;
using KaffaMaster.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaffaMaster.Models;

namespace KaffaMaster.Controllers
{
    public class BlogController : Controller
    {
        private readonly KaffaDbContext _context;

        public BlogController(KaffaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var blog = await _context.Blogs.ToListAsync();
            return View(blog);
        }
        public async Task<IActionResult> BlogDetail(int id)
        {
            var blogList = await _context.Blogs.ToListAsync();
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            BlogPageViewModel model = new()
            {
                Title = blog.Title,
                Description = blog.Description,
                Author = blog.Author,
                FamousWord = blog.FamousWord,
                Content = blog.Content,
                AuthorComment = blog.AuthorComment,


                Blogs = blogList,

            };
            return View(model);
        }

        public async Task<IActionResult> AddComment(Comment model, int blogId)
        {
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
    }
}
