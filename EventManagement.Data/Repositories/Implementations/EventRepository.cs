using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using EventManagement.Data.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Data.Repositories.Implementations;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    public EventRepository(EventManagementDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Event>> GetEventsByUserAsync(string userId)
    {
        return await _context.Registrations
            .Where(r => r.UserId == userId)
            .Select(r => r.Event)
            .ToListAsync();
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

    public async Task<IReadOnlyList<Event>> GetOrganizedEventsByUserAsync(string userId)
    {
        return await _dbSet
            .Where(e => e.OrganizerId == userId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Event>> GetPublishedEventsAsync()
    {
        return await _dbSet
            .Where(e => e.Status == EventStatus.Published)
            .ToListAsync();
    }
}
