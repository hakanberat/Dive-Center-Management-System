namespace DiveCenterManager.ViewModels;

public class CalendarReservationViewModel
{
    public int Id { get; set; }

    public DateTime ReservationDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public string ActivityName { get; set; } = string.Empty;

    public string? DiveSiteName { get; set; }

    public string? BoatName { get; set; }

    public string? StaffName { get; set; }

    public int ParticipantCount { get; set; }

    public string Status { get; set; } = string.Empty;
}