using EventManagement.Data.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Model.Entities;

public class Registration
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Confirmed;

}
