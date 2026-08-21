using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using EventManagement.Services.Dtos;
using EventManagement.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Services.Interfaces;

public interface IEventService
{
    Task<Event> CreateEventAsync(EventUpsertDto dto, string organizerId);

    Task<EventOperationResult> UpdateEventAsync(int eventId, EventUpsertDto dto, string currentUserId, bool isAdmin);

    Task<(EventOperationResult Result, Event? EventItem)> GetEditableEventAsync(int eventId, string currentUserId, bool isAdmin);
    Task<EventOperationResult> PublishEventAsync(int eventId, string currentUserId, bool isAdmin);
    Task<EventOperationResult> CancelEventAsync(int eventId, string currentUserId, bool isAdmin);
    Task<EventOperationResult> DeleteEventAsync(int eventId, string currentUserId, bool isAdmin);
    Task<PagedResult<Event>> GetMyOrganizedEventsAsync(string organizerId, int pageNumber, int pageSize);

    Task<PagedResult<Event>> GetAllEventsForAdminAsync(int pageNumber, int pageSize);
    Task<PagedResult<Event>> GetFilteredEventsAsync(int? categoryId, string? city, DateOnly? fromDate, DateOnly? toDate, int pageNumber, int pageSize);


}

