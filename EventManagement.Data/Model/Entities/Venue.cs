using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Model.Entities;

public class Venue
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;     
    public string City { get; set; } = string.Empty;
    public string? PostalCode { get; set; }


    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ICollection<Event> Events { get; set; } = [];
}
