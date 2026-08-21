using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Data.Repositories.Implementations;

public class RegistrationRepository :GenericRepository<Registration>, IRegistrationRepository
{
    public RegistrationRepository(EventManagementDbContext context) : base(context) {}

    public async Task<Registration?> GetByEventAndUserAsync(int eventId, string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
    }

    public async Task<int> GetConfirmedCountAsync(int eventId)
    {
        return await _dbSet.CountAsync(r =>
       r.EventId == eventId && r.Status == RegistrationStatus.Confirmed);
    }
    public async Task<PagedResult<Registration>> GetByUserIdAsync(string userId, int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Include(r => r.Event).ThenInclude(e => e.Category)
            .Include(r => r.Event).ThenInclude(e => e.Venue)
            .Where(r => r.UserId == userId)
             .Where(r => r.UserId == userId && r.Status != RegistrationStatus.Cancelled)
            .OrderBy(r => r.Event.StartDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Registration>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
