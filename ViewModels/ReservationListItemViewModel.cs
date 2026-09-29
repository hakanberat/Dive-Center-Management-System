namespace DiveCenterManager.ViewModels;

public class ReservationListItemViewModel
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

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public string? CurrencySymbol { get; set; }

    public string? Notes { get; set; }
}