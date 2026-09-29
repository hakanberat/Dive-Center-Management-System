namespace DiveCenterManager.ViewModels;

public class DashboardViewModel
{
    public int TodayReservationCount { get; set; }

    public int PendingReservationCount { get; set; }

    public int ConfirmedReservationCount { get; set; }

    public int ActiveStaffCount { get; set; }

    public int ActiveBoatCount { get; set; }

    public int ActiveEquipmentCount { get; set; }

    public IEnumerable<ReservationListItemViewModel> TodayReservations
        { get; set; } = [];
}