using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
        IEventRepository Events { get; }
        ICategoryRepository Categories { get; }
        IVenueRepository Venues { get; }
        IRegistrationRepository Registrations { get; }
        IOrganizerRequestRepository OrganizerRequests { get; }

    Task<int> SaveChangesAsync();
    
}
