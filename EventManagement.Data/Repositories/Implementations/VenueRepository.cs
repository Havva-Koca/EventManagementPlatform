using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Implementations;

public class VenueRepository : GenericRepository<Venue>, IVenueRepository
{
    public VenueRepository(EventManagementDbContext context) : base(context)
    {
    }
}
