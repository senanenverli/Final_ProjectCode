using Microsoft.AspNetCore.Mvc;
using KaffaMaster.Models;
using Microsoft.AspNetCore.Identity;
using KaffaMaster.Contexts;

public class SubscriptionController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public SubscriptionController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(SubscriptionViewModel subscriptionViewModel)
    {
        AppUser user = await _userManager.FindByEmailAsync(subscriptionViewModel.Email.Trim());
        if (user == null)
        {
            ModelState.AddModelError("", "istifadeci adi ve ya email yanlisdir");
            return RedirectToAction("Index", "Home");
        }

        user.IsSubscribed = true;
        IdentityResult result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return RedirectToAction("Index","Home");
    }

    public IActionResult ThankYou()
    {
        return View(); 
    }
}
