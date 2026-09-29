using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using DiveCenterManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;


[Authorize(Roles = "Admin")]
public class BoatsController : Controller
{
    private readonly BoatService _boatService;

    public BoatsController(BoatService boatService)
    {
        _boatService = boatService;
    }

  public async Task<IActionResult> Index(string? searchTerm)
{
    var boats = await _boatService.GetAllAsync(searchTerm);

    ViewBag.SearchTerm = searchTerm;

    return View(boats);
}
    [HttpGet]
public IActionResult Create()
{
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Boat boat)
{
    if (!ModelState.IsValid)
    {
        return View(boat);
    }

    await _boatService.CreateAsync(boat);

    return RedirectToAction(nameof(Index));
}
[HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var boat = await _boatService.GetByIdAsync(id);

    if (boat is null)
    {
        return NotFound();
    }

    return View(boat);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Boat boat)
{
    if (!ModelState.IsValid)
    {
        return View(boat);
    }

    var updated = await _boatService.UpdateAsync(boat);

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
    var updated = await _boatService.SetActiveStatusAsync(id, isActive);

    if (!updated)
    {
        return NotFound();
    }

    return RedirectToAction(nameof(Index));
}
}