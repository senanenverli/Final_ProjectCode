﻿using Microsoft.AspNetCore.Mvc;

namespace KaffaMaster.Controllers
{
    public class FaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
