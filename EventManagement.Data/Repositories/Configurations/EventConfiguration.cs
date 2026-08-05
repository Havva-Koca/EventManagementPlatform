using EventManagement.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength (4000);

        builder.Property(e => e.Status)
            .HasConversion<string>() 
            .HasMaxLength(20);

        // --- Relationships ---

        // If a category is deleted, its related events should not be deleted (Restrict).If this category has at least one related Event, do not allow deletion, throw an error.
        // Those events must first be moved to another category or deleted manually.
        builder.HasOne(e => e.Category)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); 

        // If this venue has at least one related Event, do not allow deletion, throw an error.
        builder.HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        // If the Organizer (ApplicationUser) is deleted, their events should not be deleted either.
        builder.HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Adding an index since list/calendar queries will frequently
        // filter/sort by StartDate.
        builder.HasIndex(e => e.StartDate);
        builder.HasIndex(e => e.Status);

    }
}
