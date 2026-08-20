using EventManagement.Services.Interfaces;
using EventManagement.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagement.Web.Controllers;

[Authorize]
public class RegistrationsController : Controller
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _registrationService.RegisterAsync(eventId, userId);

        switch (result)
        {
            case RegistrationResult.EventNotFound:
                return NotFound();
            case RegistrationResult.OwnByOrganizer:
                TempData["RegistrationMessage"] = "You can't register for your own event.";
                break;
            case RegistrationResult.EventFull:
                TempData["RegistrationMessage"] = "This event is no longer accepting registrations — it's full.";
                break;
            case RegistrationResult.AlreadyRegistered:
                TempData["RegistrationMessage"] = "You're already registered for this event.";
                break;
            case RegistrationResult.Success:
                TempData["RegistrationMessage"] = "You have successfully registered for this event!";
                break;
            default:
                TempData["RegistrationMessage"] = "An error occurred while processing your request.";
                break;
        }

        return RedirectToAction("Details", "Events", new { id = eventId });
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _registrationService.CancelAsync(eventId, userId);

        switch (result)
        {
            case RegistrationResult.RegistrationNotFound:
                return NotFound();
            case RegistrationResult.Success:
                TempData["RegistrationMessage"] = "Your registration has been cancelled.";
                break;
            default:
                TempData["RegistrationMessage"] = "An error occurred while processing your request.";
                break;
        }

        return RedirectToAction("Details", "Events", new { id = eventId });
    }
}