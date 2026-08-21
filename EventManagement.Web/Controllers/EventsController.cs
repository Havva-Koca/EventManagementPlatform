using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Services.Dtos;
using EventManagement.Services.Implementations;
using EventManagement.Services.Interfaces;
using EventManagement.Services.Results;
using EventManagement.Web.Models.EventViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EventManagement.Web.Controllers;

public class EventsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventService _eventService;
   

    public EventsController(IUnitOfWork unitOfWork, IEventService eventService)
    {
        _unitOfWork = unitOfWork;
        _eventService = eventService;
       
    }

    public async Task<IActionResult> Index(int? categoryId, string? city, DateOnly? fromDate, DateOnly? toDate)
    {
        var events = await _unitOfWork.Events.GetFilteredEventsAsync(categoryId, city, fromDate, toDate);
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var cities = await _unitOfWork.Venues.GetDistinctCitiesAsync();

        var model = new EventListViewModel
        {
            Events = events,
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList(),
            Cities = cities.Select(c => new SelectListItem { Value = c, Text = c }).ToList(),
            CategoryId = categoryId,
            City = city,
            FromDate = fromDate,
            ToDate = toDate
        };
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _unitOfWork.Events.GetEventWithDetailsAsync(id);

        if (eventItem == null)
        {
            return NotFound();
        }
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (eventItem.Status == EventStatus.Draft &&
            !isAdmin && eventItem.OrganizerId != currentUserId)
        {
            return NotFound();
        }
        var confirmedCount = eventItem.Registrations.Count(r => r.Status == RegistrationStatus.Confirmed);
        var model = new EventDetailsViewModel
        {
            EventItem = eventItem,
            IsOwnEvent = eventItem.OrganizerId == currentUserId,
            IsRegistered = eventItem.Registrations.Any(r =>
                r.UserId == currentUserId && r.Status == RegistrationStatus.Confirmed),
            IsFull = confirmedCount >= eventItem.Capacity,
            ConfirmedCount = confirmedCount
        };

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Create()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();

        var model = new CreateEventViewModel
        {
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList(),

            TimeSlots = GenerateTimeSlots()
        };

        return View(model);
    }

    private static List<SelectListItem> GenerateTimeSlots()
    {
        var slots = new List<SelectListItem>();

        for (int hour = 0; hour < 24; hour++)
        {
            foreach (var minute in new[] { 0, 30 })
            {
                var time = new TimeOnly(hour, minute);
                var timeText = time.ToString("HH:mm");

                slots.Add(new SelectListItem
                {
                    Value = timeText,
                    Text = timeText
                });
            }
        }

        return slots;
    }

    [HttpPost]
    [Authorize(Roles = "Organizer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEventViewModel model)
    {
        var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (organizerId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            model.TimeSlots = GenerateTimeSlots();

            return View(model);
        }

        var startTime = TimeOnly.Parse(model.StartTime!);
        var endTime = TimeOnly.Parse(model.EndTime!);

        var dto = new EventUpsertDto
        {
            Title = model.Title,
            Description = model.Description,
            StartDate = model.StartDate!.Value.ToDateTime(startTime),
            EndDate = model.EndDate!.Value.ToDateTime(endTime),
            Capacity = model.Capacity,
            CategoryId = model.CategoryId,
            VenueName = model.VenueName,
            VenueStreet = model.VenueStreet,
            VenueCity = model.VenueCity,
            VenuePostalCode = model.VenuePostalCode
        };

        var newEvent = await _eventService.CreateEventAsync(dto, organizerId);

        return RedirectToAction("Details", new { id = newEvent.Id });
    }

    [HttpGet]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var (result, eventItem) = await _eventService.GetEditableEventAsync(id, currentUserId!, isAdmin);

        if (result == EventOperationResult.NotFound)
        {
            return NotFound();
        }

        if (result == EventOperationResult.Forbidden)
        {
            return Forbid();
        }

        var categories = await _unitOfWork.Categories.GetAllAsync();

        var model = new EditEventViewModel
        {
            EventId = eventItem!.Id,
            Title = eventItem.Title,
            Description = eventItem.Description,
            StartDate = DateOnly.FromDateTime(eventItem.StartDate),
            StartTime = TimeOnly.FromDateTime(eventItem.StartDate).ToString("HH:mm"),
            EndDate = DateOnly.FromDateTime(eventItem.EndDate),
            EndTime = TimeOnly.FromDateTime(eventItem.EndDate).ToString("HH:mm"),
            Capacity = eventItem.Capacity,
            CategoryId = eventItem.CategoryId,
            VenueName = eventItem.Venue.Name,
            VenueStreet = eventItem.Venue.Street,
            VenueCity = eventItem.Venue.City,
            VenuePostalCode = eventItem.Venue.PostalCode,
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList(),
            TimeSlots = GenerateTimeSlots()
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer,Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditEventViewModel model)
    {
        if (id != model.EventId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            model.TimeSlots = GenerateTimeSlots();

            return View(model);
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var startTime = TimeOnly.Parse(model.StartTime!);
        var endTime = TimeOnly.Parse(model.EndTime!);

        var dto = new EventUpsertDto
        {
            Title = model.Title,
            Description = model.Description,
            StartDate = model.StartDate!.Value.ToDateTime(startTime),
            EndDate = model.EndDate!.Value.ToDateTime(endTime),
            Capacity = model.Capacity,
            CategoryId = model.CategoryId,
            VenueName = model.VenueName,
            VenueStreet = model.VenueStreet,
            VenueCity = model.VenueCity,
            VenuePostalCode = model.VenuePostalCode
        };

        var result = await _eventService.UpdateEventAsync(id, dto, currentUserId!, isAdmin);

        if (result == EventOperationResult.NotFound)
        {
            return NotFound();
        }

        if (result == EventOperationResult.Forbidden)
        {
            return Forbid();
        }
        
        TempData["EventMessage"] = "Event updated successfully.";
 
        return RedirectToAction("Details", new { id });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _eventService.PublishEventAsync(id, currentUserId!, isAdmin);

        switch (result)
        {
            case EventOperationResult.NotFound:
                return NotFound();
            case EventOperationResult.Forbidden:
                return Forbid();
            default:
                TempData["EventMessage"] = "Event published successfully.";
                return RedirectToAction("Index", "MyEvents");
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _eventService.CancelEventAsync(id, currentUserId!, isAdmin);

        switch (result)
        {
            case EventOperationResult.NotFound:
                return NotFound();
            case EventOperationResult.Forbidden:
                return Forbid();
            default:
                TempData["EventMessage"] = "Event cancelled successfully.";
                return RedirectToAction("Index", "MyEvents");
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var result = await _eventService.DeleteEventAsync(id, currentUserId!, isAdmin);

        switch (result)
        {
            case EventOperationResult.NotFound:
                return NotFound();
            case EventOperationResult.Forbidden:
                return Forbid();
            default:
                TempData["EventMessage"] = "Event deleted successfully.";
                return RedirectToAction("Index", "MyEvents");
        }
    }
   
}