using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Implementations;

public class VenueRepository : GenericRepository<Venue>, IVenueRepository
{
    public VenueRepository(EventManagementDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<string>> GetDistinctCitiesAsync()
    {
        return await _dbSet
            .Select(v =>v.City)
            .Distinct()
            .OrderBy(c=>c)
            .ToListAsync();
    }
}
