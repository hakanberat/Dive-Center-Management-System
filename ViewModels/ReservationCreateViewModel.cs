using System.ComponentModel.DataAnnotations;
using DiveCenterManager.Models;

namespace DiveCenterManager.ViewModels;

public class ReservationCreateViewModel
{
    [Required(ErrorMessage = "Reservation date is required.")]
    public DateTime ReservationDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Start time is required.")]
    public TimeSpan StartTime { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Activity is required.")]
    public int ActivityId { get; set; }

    public int? DiveSiteId { get; set; }

    public int? BoatId { get; set; }

    public int? StaffId { get; set; }

    [Range(
        1,
        500,
        ErrorMessage = "Participant count must be greater than zero.")]
    public int ParticipantCount { get; set; } = 1;

    [StringLength(1000)]
    public string? Notes { get; set; }

    public IEnumerable<DiveActivity> AvailableActivities { get; set; }
        = [];

    public IEnumerable<DiveSite> AvailableDiveSites { get; set; }
        = [];

    public IEnumerable<Boat> AvailableBoats { get; set; }
        = [];

    public IEnumerable<Staff> AvailableStaff { get; set; }
        = [];
}