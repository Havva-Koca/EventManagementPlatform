using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Services.Implementations;
using EventManagement.Services.Interfaces;
using EventManagement.Services.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOrganizerRequestService _organizerRequestService;
    private readonly IEventService _eventService;

    public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IOrganizerRequestService organizerRequestService, IEventService eventService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _organizerRequestService = organizerRequestService;
        _eventService = eventService;
    }
    public async Task<IActionResult> PendingRequests()
    {
        var pendingRequests = await _unitOfWork.OrganizerRequests.GetPendingRequestsWithUserAsync();
        return View(pendingRequests);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var reviewerId = _userManager.GetUserId(User)!;
        var result = await _organizerRequestService.ApproveRequestAsync(id, reviewerId);

        if (result == OrganizerRequestResult.NotFound)
        {
            return NotFound();
        }

        return RedirectToAction("PendingRequests");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectRequest(int id)
    {
        var reviewerId = _userManager.GetUserId(User)!;
        var result = await _organizerRequestService.RejectRequestAsync(id, reviewerId);

        if (result == OrganizerRequestResult.NotFound)
        {
            return NotFound();
        }

        return RedirectToAction("PendingRequests");
    }
    public IActionResult Index()
    {
        return View();
    }
    private const int PageSize = 6;

    [HttpGet]
    public async Task<IActionResult> Events(int page = 1)
    {
        var events = await _eventService.GetAllEventsForAdminAsync(page, PageSize);
        return View(events);
    }
}