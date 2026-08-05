using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Model.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;


    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Event> OrganizedEvents  { get; set; } = new List<Event>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
