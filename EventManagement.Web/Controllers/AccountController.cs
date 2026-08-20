using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            FullName = model.FullName,
            Email = model.Email,
            UserName = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Participant");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Events");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Events");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Events");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestOrganizerRole()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login");
        }

        // Check if the user already has a pending request,
        // to prevent them from submitting duplicate requests.
        var existingRequests = await _unitOfWork.OrganizerRequests.GetAllAsync();
        bool hasPendingRequest = existingRequests.Any(r =>
            r.UserId == userId && r.Status == RequestStatus.Pending);

        if (hasPendingRequest)
        {
            TempData["Message"] = "You already have a pending request.";
            return RedirectToAction("Index", "Home");
        }

        var request = new OrganizerRequest
        {
            UserId = userId,
            RequestedAt = DateTime.UtcNow,
            Status = RequestStatus.Pending
        };

        await _unitOfWork.OrganizerRequests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        TempData["Message"] = "Your request has been submitted.";
        return RedirectToAction("Index", "Events");
    }
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }


}
