using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using DiveCenterManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;
[Authorize(Roles = "Admin")]
public class DiveSitesController : Controller
{
    private readonly DiveSiteService _diveSiteService;

    public DiveSitesController(DiveSiteService diveSiteService)
    {
        _diveSiteService = diveSiteService;
    }

 public async Task<IActionResult> Index(string? searchTerm)
{
    var diveSites = await _diveSiteService.GetAllAsync(searchTerm);

    ViewBag.SearchTerm = searchTerm;

    return View(diveSites);
}
    [HttpGet]
public IActionResult Create()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(DiveSite diveSite)
{
    if (!ModelState.IsValid)
    {
        return View(diveSite);
    }

    await _diveSiteService.CreateAsync(diveSite);

    return RedirectToAction(nameof(Index));
}
[HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var diveSite = await _diveSiteService.GetByIdAsync(id);

    if (diveSite is null)
    {
        return NotFound();
    }

    return View(diveSite);
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(DiveSite diveSite)
{
    if (!ModelState.IsValid)
    {
        return View(diveSite);
    }

    var updated = await _diveSiteService.UpdateAsync(diveSite);

    if (!updated)
    {
        return NotFound();
    }

    return RedirectToAction(nameof(Index));
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SetActiveStatus(int id, bool isActive)
{
    var updated = await _diveSiteService.SetActiveStatusAsync(id, isActive);

    if (!updated)
    {
        return NotFound();
    }

    return RedirectToAction(nameof(Index));
}

}