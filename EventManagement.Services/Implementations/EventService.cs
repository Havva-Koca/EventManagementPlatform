using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Services.Dtos;
using EventManagement.Services.Interfaces;
using EventManagement.Services.Results;

namespace EventManagement.Services.Implementations;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Event> CreateEventAsync(EventUpsertDto dto, string organizerId)
    {
        var venue = await ResolveVenueAsync(dto.VenueName, dto.VenueStreet, dto.VenueCity, dto.VenuePostalCode);

        var newEvent = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Capacity = dto.Capacity,
            CategoryId = dto.CategoryId,
            VenueId = venue.Id,
            OrganizerId = organizerId,
            Status = EventStatus.Published
        };

        await _unitOfWork.Events.AddAsync(newEvent);
        await _unitOfWork.SaveChangesAsync();

        return newEvent;
    }

    public async Task<EventOperationResult> UpdateEventAsync(int eventId, EventUpsertDto dto, string currentUserId, bool isAdmin)
    {
        var eventItem = await _unitOfWork.Events.GetByIdAsync(eventId);

        if (eventItem == null)
        {
            return EventOperationResult.NotFound;
        }

        if (!isAdmin && eventItem.OrganizerId != currentUserId)
        {
            return EventOperationResult.Forbidden;
        }

        var venue = await ResolveVenueAsync(dto.VenueName, dto.VenueStreet, dto.VenueCity, dto.VenuePostalCode);

        eventItem.Title = dto.Title;
        eventItem.Description = dto.Description;
        eventItem.StartDate = dto.StartDate;
        eventItem.EndDate = dto.EndDate;
        eventItem.Capacity = dto.Capacity;
        eventItem.CategoryId = dto.CategoryId;
        eventItem.VenueId = venue.Id;

        await _unitOfWork.SaveChangesAsync();

        return EventOperationResult.Success;
    }

    private async Task<Venue> ResolveVenueAsync(string name, string street, string city, string? postalCode)
    {
        var existingVenues = await _unitOfWork.Venues.FindAsync(v =>
            v.Name == name && v.Street == street && v.City == city && v.PostalCode == postalCode);

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

    public async Task<(EventOperationResult Result, Event? EventItem)> GetEditableEventAsync(int eventId, string currentUserId, bool isAdmin)
    {
        var eventItem = await _unitOfWork.Events.GetEventWithDetailsAsync(eventId);

        if (eventItem == null)
        {
            return (EventOperationResult.NotFound, null);
        }

        if (!isAdmin && eventItem.OrganizerId != currentUserId)
        {
            return (EventOperationResult.Forbidden, null);
        }

        return (EventOperationResult.Success, eventItem);
    }
}