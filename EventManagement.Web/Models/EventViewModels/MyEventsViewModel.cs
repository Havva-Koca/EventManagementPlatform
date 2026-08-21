using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;

namespace EventManagement.Web.Models.EventViewModels;

public class MyEventsViewModel
{
    public PagedResult<Event> OrganizedEvents { get; set; } = new();
    public PagedResult<Registration> RegisteredEvents { get; set; } = new();
    public string ActiveTab { get; set; } = "organized";
}
