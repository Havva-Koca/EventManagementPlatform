using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventManagement.Web.Models.EventViewModels;

public class CreateEventViewModel :IValidatableObject
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;



    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateOnly? StartDate { get; set; }

    [Required]
    [Display(Name = "Start Time")]
    public string? StartTime { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateOnly? EndDate { get; set; }

    [Required]
    [Display(Name = "End Time")]
    public string? EndTime { get; set; }

    [Required]
    [Range(1, 10000)]
    public int Capacity { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    [Display(Name = "Venue Name")]
    [StringLength(100)]
    public string VenueName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Street Address")]
    [StringLength(500)]
    public string VenueStreet { get; set; } = string.Empty;

    [Required]
    [Display(Name = "City")]
    [StringLength(100)]
    public string VenueCity { get; set; } = string.Empty;

    [Display(Name = "Postal Code")]
    [StringLength(20)]
    public string? VenuePostalCode { get; set; }

    public List<SelectListItem>? Categories { get; set; }
    public List<SelectListItem>? TimeSlots { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (StartDate.HasValue && EndDate.HasValue &&
            !string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
        {
            var startDateTime = StartDate.Value.ToDateTime(TimeOnly.Parse(StartTime));
            var endDateTime = EndDate.Value.ToDateTime(TimeOnly.Parse(EndTime));

            if (endDateTime <= startDateTime)
            {
                errors.Add(new ValidationResult(
                    "End date/time must be after the start date/time.",
                    [ nameof(EndDate), nameof(EndTime) ]));
            }
        }

        return errors;
    }
}
