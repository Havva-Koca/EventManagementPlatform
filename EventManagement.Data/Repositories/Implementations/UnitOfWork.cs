using EventManagement.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventManagementDbContext _context;
    public UnitOfWork(EventManagementDbContext context)
    {
        _context = context;
        Events = new EventRepository(_context);
        Categories = new CategoryRepository(_context);
        Venues = new VenueRepository(_context);
        Registrations = new RegistrationRepository(_context);
    }
    public IEventRepository Events { get; }

    public ICategoryRepository Categories { get; }

    public IVenueRepository Venues { get; }

    public IRegistrationRepository Registrations { get; }

    public void Dispose()
    {
        _context.Dispose();
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
