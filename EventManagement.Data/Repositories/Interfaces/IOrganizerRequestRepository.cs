using EventManagement.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Interfaces;

public interface IOrganizerRequestRepository :IGenericRepository<OrganizerRequest>
{
    Task<List<OrganizerRequest>> GetPendingRequestsWithUserAsync();
}
