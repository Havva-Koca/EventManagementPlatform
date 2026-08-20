using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Web.Models.EventViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EventManagement.Web.Controllers;

public class EventsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public EventsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    private async Task<Venue> ResolveVenueAsync(string name, string street, string city, string? postalCode)
    {
        var existingVenues = await _unitOfWork.Venues.FindAsync(v =>
            v.Name == name && v.City == city);

        if (existingVenues.Count > 0)
        {
            return existingVenues[0];
        }

        var venue = new Venue
        {
            Name = name,
            Street = street,
            City = city,
            PostalCode = postalCode
        };

        await _unitOfWork.Venues.AddAsync(venue);
        await _unitOfWork.SaveChangesAsync();

        return venue;
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

        var startDateTime = model.StartDate!.Value.ToDateTime(startTime);
        var endDateTime = model.EndDate!.Value.ToDateTime(endTime);

        var venue = await ResolveVenueAsync(model.VenueName, model.VenueStreet, model.VenueCity, model.VenuePostalCode);

        var newEvent = new Event
        {
            Title = model.Title,
            Description = model.Description,
            StartDate = startDateTime,
            EndDate = endDateTime,
            Capacity = model.Capacity,
            CategoryId = model.CategoryId,
            VenueId = venue.Id,
            OrganizerId = organizerId,
            Status = EventStatus.Published
        };

        await _unitOfWork.Events.AddAsync(newEvent);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Details", new { id = newEvent.Id });
    }
    [HttpGet]
    [Authorize(Roles = "Organizer,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var eventItem = await _unitOfWork.Events.GetEventWithDetailsAsync(id);

        if (eventItem == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && eventItem.OrganizerId != currentUserId)
        {
            return Forbid();
        }

        var categories = await _unitOfWork.Categories.GetAllAsync();

        var model = new EditEventViewModel
        {
            EventId = eventItem.Id,
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

        var eventItem = await _unitOfWork.Events.GetByIdAsync(id);

        if (eventItem == null)
        {
            return NotFound();
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && eventItem.OrganizerId != currentUserId)
        {
            return Forbid();
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

        var startDateTime = model.StartDate!.Value.ToDateTime(startTime);
        var endDateTime = model.EndDate!.Value.ToDateTime(endTime);

        var venue = await ResolveVenueAsync(model.VenueName, model.VenueStreet, model.VenueCity, model.VenuePostalCode);

        eventItem.Title = model.Title;
        eventItem.Description = model.Description;
        eventItem.StartDate = startDateTime;
        eventItem.EndDate = endDateTime;
        eventItem.Capacity = model.Capacity;
        eventItem.CategoryId = model.CategoryId;
        eventItem.VenueId = venue.Id;

        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Details", new { id = eventItem.Id });
    }


}
