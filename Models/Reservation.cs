using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Reservation date is required.")]
    public DateTime ReservationDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Start time is required.")]
    public TimeSpan StartTime { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Activity is required.")]
    public int ActivityId { get; set; }

    public int? DiveSiteId { get; set; }

    public int? BoatId { get; set; }

    public int? StaffId { get; set; }

    [Range(1, 500, ErrorMessage = "Participant count must be greater than zero.")]
    public int ParticipantCount { get; set; } = 1;

    [Required]
    [RegularExpression(
        "^(Pending|Confirmed|Completed|Cancelled|NoShow)$",
        ErrorMessage = "Invalid reservation status.")]
    public string Status { get; set; } = "Pending";

    [Range(0, 9999999, ErrorMessage = "Unit price cannot be negative.")]
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Currency is required.")]
    public int CurrencyId { get; set; }

    [Range(0, 999999999, ErrorMessage = "Total amount cannot be negative.")]
    public decimal TotalAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}