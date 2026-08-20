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
}
