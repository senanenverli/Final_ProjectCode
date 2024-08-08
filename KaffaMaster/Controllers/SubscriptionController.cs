using Microsoft.AspNetCore.Mvc;
using KaffaMaster.Models; 

public class SubscriptionController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Subscribe(SubscriptionViewModel subscriptionViewModel)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("ThankYou");
        }

        return RedirectToAction("Index", "Home"); 
    }

    public IActionResult ThankYou()
    {
        return View(); 
    }
}
