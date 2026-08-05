using EventManagement.Data.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Model.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } =string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Category
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    //Venue
    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
    //ApplicationUser (Organizer)
    public string OrganizerId { get; set; } = string.Empty;
    public ApplicationUser Organizer { get; set; } = null!;

    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();


}
