using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventManagement.Data.Model.Entities;

namespace EventManagement.Data.Repositories.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.Property(v => v.Name)
             .IsRequired()
             .HasMaxLength(100);

        builder.Property(v => v.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Street)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.PostalCode)
            .HasMaxLength(20)
            .IsRequired(false);



    }
}
