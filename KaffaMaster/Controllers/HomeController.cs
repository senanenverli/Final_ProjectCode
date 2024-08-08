using KaffaMaster.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace KaffaMaster.Controllers;

public class HomeController : Controller
{
	private readonly KaffaDbContext _context;

	public HomeController(KaffaDbContext context)
	{
		_context = context;
	}


	public IActionResult Index()
	{
		return View();
	}

	

}