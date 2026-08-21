using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.Web.Models.EventViewModels;

public class EventListViewModel
{
    public PagedResult<Event> Events { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = [];
    public List<SelectListItem> Cities { get; set; } = [];
    public int? CategoryId { get; set; }
    public string? City { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}
