using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Implementations;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(EventManagementDbContext context) : base(context)
    {
    }

 

    public async Task<Event?> GetEventWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

 

    public async Task<IReadOnlyList<Event>> GetPublishedEventsAsync()
    {
        return await _dbSet
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Where(e => e.Status == EventStatus.Published)
            .ToListAsync();
    }

    public async Task<PagedResult<Event>> GetFilteredEventsAsync(
        int? categoryId, string? city, DateOnly? fromDate, DateOnly? toDate,
        int pageNumber, int pageSize)
    {
        var query = _dbSet
             .Include(e => e.Category)
             .Include(e => e.Venue)
             .Where(e => e.Status == EventStatus.Published)
             .Where(e => e.EndDate >= DateTime.Now);

        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(e => e.Venue.City == city);

        if (fromDate.HasValue)
            query = query.Where(e => e.StartDate >= fromDate.Value.ToDateTime(TimeOnly.MinValue));

        if (toDate.HasValue)
            query = query.Where(e => e.StartDate <= toDate.Value.ToDateTime(TimeOnly.MaxValue));

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Event>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<Event>> GetByOrganizerIdAsync(string organizerId, int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Where(e => e.OrganizerId == organizerId)
            .OrderByDescending(e => e.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Event>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    public async Task<PagedResult<Event>> GetAllEventsWithDetailsAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.Registrations)
            .OrderByDescending(e => e.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Event>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }


}
