using DiveCenterManager.Models;
using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class ActivitiesController : Controller
{
    private readonly ActivityService _activityService;
    private readonly CurrencyService _currencyService;

    public ActivitiesController(
        ActivityService activityService,
        CurrencyService currencyService)
    {
        _activityService = activityService;
        _currencyService = currencyService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
{
    var activities = await _activityService.GetAllAsync(searchTerm);

    ViewBag.SearchTerm = searchTerm;

    return View(activities);
}

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCurrenciesAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DiveActivity activity)
    {
        if (!ModelState.IsValid)
        {
            await LoadCurrenciesAsync(activity.CurrencyId);

            return View(activity);
        }

        await _activityService.CreateAsync(activity);

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var activity = await _activityService.GetByIdAsync(id);

    if (activity is null)
    {
        return NotFound();
    }

    await LoadCurrenciesAsync(activity.CurrencyId);

    return View(activity);
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(DiveActivity activity)
{
    if (!ModelState.IsValid)
    {
        await LoadCurrenciesAsync(activity.CurrencyId);

        return View(activity);
    }

    var updated = await _activityService.UpdateAsync(activity);

    if (!updated)
    {
        return NotFound();
    }

    return RedirectToAction(nameof(Index));
}

    private async Task LoadCurrenciesAsync(int? selectedCurrencyId = null)
    {
        var currencies = await _currencyService.GetActiveAsync();

        ViewBag.Currencies = new SelectList(
            currencies,
            "Id",
            "Code",
            selectedCurrencyId);
    }
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SetActiveStatus(int id, bool isActive)
{
    var updated = await _activityService.SetActiveStatusAsync(id, isActive);

    if (!updated)
    {
        return NotFound();
    }

    return RedirectToAction(nameof(Index));
}
}