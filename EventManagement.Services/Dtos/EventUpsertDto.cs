using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Services.Dtos;

public class EventUpsertDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public int CategoryId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string VenueStreet { get; set; } = string.Empty;
    public string VenueCity { get; set; } = string.Empty;
    public string? VenuePostalCode { get; set; }
}
