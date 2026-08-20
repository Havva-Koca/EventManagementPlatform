using EventManagement.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using EventManagement.Data.Model.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Configurations;

public class OrganizerRequestConfiguration : IEntityTypeConfiguration<OrganizerRequest>
{
    public void Configure(EntityTypeBuilder<OrganizerRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        // A user's relationship with their OrganizerRequests.
        // DeleteBehavior.Cascade: if the user is deleted, their requests are deleted too.

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();  

        builder.Property(r => r.ReviewedByUserId)
            .IsRequired(false);
    }
}
