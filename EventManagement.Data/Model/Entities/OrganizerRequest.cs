using EventManagement.Data.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Model.Entities;

public class OrganizerRequest
{
    public int Id { get; set; }

 
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public RequestStatus Status { get; set; } = RequestStatus.Pending;


    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByUserId { get; set; }
}
