using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly ReportsService _reportsService;

    public ReportsController(
        ReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    public async Task<IActionResult> Index(
        DateTime? startDate,
        DateTime? endDate)
    {
        var today = DateTime.Today;

        var selectedStartDate =
            startDate ?? new DateTime(today.Year, today.Month, 1);

        var selectedEndDate =
            endDate ?? today;

        if (selectedEndDate < selectedStartDate)
        {
            ModelState.AddModelError(
                string.Empty,
                "End date cannot be earlier than start date.");

            selectedEndDate = selectedStartDate;
        }

        var model =
            await _reportsService.GetReportAsync(
                selectedStartDate,
                selectedEndDate);

        return View(model);
    }
}