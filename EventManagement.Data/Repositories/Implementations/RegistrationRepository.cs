using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;

namespace EventManagement.Data.Repositories.Implementations;

public class RegistrationRepository :GenericRepository<Registration>, IRegistrationRepository
{
    public RegistrationRepository(EventManagementDbContext context) : base(context) {}

    public async Task<Registration?> GetByEventAndUserAsync(int eventId, string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
    }
}
