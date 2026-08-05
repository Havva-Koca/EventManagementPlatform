using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories;

public class EventManagementDbContext :IdentityDbContext<ApplicationUser>
{
    public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options)
       : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
       
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new VenueConfiguration());
        builder.ApplyConfiguration(new EventConfiguration());
        builder.ApplyConfiguration(new RegistrationConfiguration());
    }
}
