using EventManagement.Data.Model.Entities;

namespace EventManagement.Web.Models.EventViewModels;

public class MyEventsViewModel
{
    public List<Event> OrganizedEvents { get; set; } = [];
    public List<Registration> RegisteredEvents { get; set; } = [];
}
