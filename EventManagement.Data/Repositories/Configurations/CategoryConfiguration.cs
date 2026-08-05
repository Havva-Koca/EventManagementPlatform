using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventManagement.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name)
             .IsRequired()
             .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        
        builder.HasIndex(c => c.Name).IsUnique();
    }
}
