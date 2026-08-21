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

    private const int PageSize = 6;

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index(int orgPage = 1, int regPage = 1, string tab = "organized")
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var viewModel = new MyEventsViewModel
        {
            OrganizedEvents = await _eventService.GetMyOrganizedEventsAsync(userId, orgPage, PageSize),
            RegisteredEvents = await _registrationService.GetMyRegistrationsAsync(userId, regPage, PageSize),
            ActiveTab = tab
        };

        return View(viewModel);
    }
}

