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
    public async Task<IActionResult> Index()
    {
        var events = await _unitOfWork.Events.GetPublishedEventsAsync();
        return View(events);
    }
    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _unitOfWork.Events.GetEventWithDetailsAsync(id);

        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
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

        var existingVenues = await _unitOfWork.Venues.FindAsync(v =>
            v.Name == model.VenueName && v.City == model.VenueCity);

        Venue venue;
        if (existingVenues.Count > 0)
        {
            venue = existingVenues[0];
        }
        else
        {
            venue = new Venue
            {
                Name = model.VenueName,
                Street = model.VenueStreet,
                City = model.VenueCity,
                PostalCode = model.VenuePostalCode
            };

            await _unitOfWork.Venues.AddAsync(venue);
            await _unitOfWork.SaveChangesAsync();
        }

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
}
