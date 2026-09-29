using DiveCenterManager.Models;
using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class EquipmentController : Controller
{
    private readonly EquipmentService _equipmentService;

    public EquipmentController(EquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        var equipment = await _equipmentService.GetAllAsync(searchTerm);

        ViewBag.SearchTerm = searchTerm;

        return View(equipment);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Equipment());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Equipment equipment)
    {
        if (!ModelState.IsValid)
        {
            return View(equipment);
        }

        await _equipmentService.CreateAsync(equipment);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var equipment = await _equipmentService.GetByIdAsync(id);

        if (equipment is null)
        {
            return NotFound();
        }

        return View(equipment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Equipment equipment)
    {
        if (!ModelState.IsValid)
        {
            return View(equipment);
        }

        await _equipmentService.UpdateAsync(equipment);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActiveStatus(int id)
    {
        await _equipmentService.ToggleActiveStatusAsync(id);

        return RedirectToAction(nameof(Index));
    }
}