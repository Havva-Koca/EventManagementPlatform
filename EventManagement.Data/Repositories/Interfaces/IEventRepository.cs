using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
namespace EventManagement.Data.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IReadOnlyList<Event>> GetPublishedEventsAsync();
    Task<Event?> GetEventWithDetailsAsync(int id);
    Task<PagedResult<Event>> GetFilteredEventsAsync(int? categoryId, string? city, DateOnly? fromDate, DateOnly? toDate,int pageNumber, int pageSize);
    Task<PagedResult<Event>> GetByOrganizerIdAsync(string organizerId, int pageNumber, int pageSize);
    Task<PagedResult<Event>> GetAllEventsWithDetailsAsync(int pageNumber, int pageSize);
}
