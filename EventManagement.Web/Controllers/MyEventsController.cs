using EventManagement.Services.Interfaces;
using EventManagement.Web.Models.EventViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagement.Web.Controllers;

public class MyEventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly IRegistrationService _registrationService;

    public MyEventsController(IEventService eventService, IRegistrationService registrationService)
    {
        _eventService = eventService;
        _registrationService = registrationService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var viewModel = new MyEventsViewModel
        {
            OrganizedEvents = await _eventService.GetMyOrganizedEventsAsync(userId),
            RegisteredEvents = await _registrationService.GetMyRegistrationsAsync(userId)
        };

        return View(viewModel);
    }
}

