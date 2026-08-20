using EventManagement.Data.Model.Entities;

namespace EventManagement.Web.Models.EventViewModels;

public class EventDetailsViewModel
{
    public Event EventItem { get; set; } = null!;
    public bool IsRegistered { get; set; }
    public bool IsFull { get; set; }
    public bool IsOwnEvent { get; set; }
    public int ConfirmedCount { get; set; }

}
