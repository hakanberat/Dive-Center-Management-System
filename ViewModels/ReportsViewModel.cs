namespace DiveCenterManager.ViewModels;

public class ReportsViewModel
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int ReservationCount { get; set; }

    public int ParticipantCount { get; set; }

    public IEnumerable<ReportCurrencyTotalViewModel> IncomeTotals
        { get; set; } = [];

    public IEnumerable<ReportCurrencyTotalViewModel> ExpenseTotals
        { get; set; } = [];

    public IEnumerable<ReportStatusViewModel> ReservationStatuses
        { get; set; } = [];

    public IEnumerable<ReportActivityViewModel> Activities
        { get; set; } = [];
}

public class ReportCurrencyTotalViewModel
{
    public string CurrencyCode { get; set; } = string.Empty;

    public string? CurrencySymbol { get; set; }

    public decimal TotalAmount { get; set; }
}

public class ReportStatusViewModel
{
    public string Status { get; set; } = string.Empty;

    public int Count { get; set; }
}

public class ReportActivityViewModel
{
    public string ActivityName { get; set; } = string.Empty;

    public int ReservationCount { get; set; }

    public int ParticipantCount { get; set; }
}