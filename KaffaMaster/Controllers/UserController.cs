using Helpers;
using KaffaMaster.Contexts;
using KaffaMaster.Helpers.Enums;
using KaffaMaster.Models;
using KaffaMaster.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KaffaMaster.Controllers;

public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly KaffaDbContext _context;
    public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, KaffaDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(AppUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        AppUser appUser = new AppUser()
        {
            //Fullname = model.UserName,
            Email = model.Email,
            UserName = model.UserName,
            IsActive = true
        };

        IdentityResult identityResult = await _userManager.CreateAsync(appUser, model.Password);
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        string link = Url.Action("ConfirmEmail", "Auth", new { email = appUser.Email, token }, HttpContext.Request.Scheme, HttpContext.Request.Host.Value);

        string body = $"<a href='{link}'>Confirm your email</a>";

        EmailHelper emailHelper = new EmailHelper(_configuration);
        await emailHelper.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Confirm Email", Body = body });


        await _userManager.AddToRoleAsync(appUser, Roles.Moderator.ToString());


        return RedirectToAction("Login", "Auth");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        TempData["Tab"] = "account-dashboard";


        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null)
            return NotFound();

        UserUpdateViewModel userUpdateViewModel = new()
        {
            //	Fullname = user.Fullname,
            UserName = user.UserName,
            Email = user.Email
        };


        return View(userUpdateViewModel);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UserUpdateViewModel userUpdateProfileViewModel)
    {
        TempData["Tab"] = "account-details";
        if (!ModelState.IsValid)
            return View(nameof(Profile), userUpdateProfileViewModel);

        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null)
            return NotFound();


        if (user.UserName != userUpdateProfileViewModel.UserName && _userManager.Users.Any(u => u.UserName == userUpdateProfileViewModel.UserName))
        {
            ModelState.AddModelError("UserName", "Bele username var");
            return View(nameof(Profile), userUpdateProfileViewModel);
        }

        if (user.Email != userUpdateProfileViewModel.Email && _userManager.Users.Any(u => u.Email == userUpdateProfileViewModel.Email))
        {
            ModelState.AddModelError("Email", "Bele email var");
            return View(nameof(Profile), userUpdateProfileViewModel);
        }

        if (userUpdateProfileViewModel.CurrentPassword != null)
        {
            if (userUpdateProfileViewModel.NewPassword == null)
            {
                ModelState.AddModelError("NewPassword", "Yeni password null ola bilmez");
                return View(nameof(Profile), userUpdateProfileViewModel);
            }

            IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, userUpdateProfileViewModel.CurrentPassword, userUpdateProfileViewModel.NewPassword);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(nameof(Profile), userUpdateProfileViewModel);
            }
        }

        //user.Fullname = userUpdateProfileViewModel.Fullname;
        user.UserName = userUpdateProfileViewModel.UserName;
        user.NormalizedUserName = userUpdateProfileViewModel.UserName.ToUpper();
        IdentityResult result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(nameof(Profile), userUpdateProfileViewModel);
        }

        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "Sizin profiliniz ugurla yenilendi";

        return RedirectToAction(nameof(Profile));
    }
}

