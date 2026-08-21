using System;
using System.Collections.Generic;
using System.Text;
using EventManagement.Data.Model.Entities;
namespace EventManagement.Data.Repositories.Interfaces;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<IReadOnlyList<Event>> GetPublishedEventsAsync();
    Task<Event?> GetEventWithDetailsAsync(int id);  
   
    
    Task<IReadOnlyList<Event>>GetFilteredEventsAsync(int? categoryId, string? city,DateOnly? fromDate, DateOnly? toDate);
    Task<List<Event>> GetByOrganizerIdAsync(string organizerId);
    Task<List<Event>> GetAllEventsWithDetailsAsync();
}
