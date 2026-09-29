using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiveCenterManager.Controllers;

public class CalendarController : Controller
{
    private readonly ReservationService _reservationService;

    public CalendarController(
        ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<IActionResult> Index(
        int? year,
        int? month)
    {
        var today = DateTime.Today;

        var selectedYear =
            year ?? today.Year;

        var selectedMonth =
            month ?? today.Month;

        if (selectedMonth < 1 || selectedMonth > 12)
        {
            selectedMonth = today.Month;
        }

        var reservations =
            await _reservationService.GetCalendarAsync(
                selectedYear,
                selectedMonth);

        ViewBag.Year = selectedYear;
        ViewBag.Month = selectedMonth;

        return View(reservations);
    }
}