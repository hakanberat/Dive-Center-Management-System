using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{

    private readonly DashboardService _dashboardService;

    public DashboardController(
        DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var model =
            await _dashboardService.GetDashboardAsync();

        return View(model);
    }
}