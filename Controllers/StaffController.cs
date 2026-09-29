using DiveCenterManager.Services;
using DiveCenterManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class StaffController : Controller
{
    private readonly StaffService _staffService;
    private readonly OperationalRoleService _roleService;

    public StaffController(
        StaffService staffService,
        OperationalRoleService roleService)
    {
        _staffService = staffService;
        _roleService = roleService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
{
    var staff = await _staffService.GetAllAsync(searchTerm);

    ViewBag.SearchTerm = searchTerm;

    return View(staff);
}

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new StaffCreateViewModel
        {
            AvailableRoles = await _roleService.GetActiveAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await _roleService.GetActiveAsync();

            return View(model);
        }

        await _staffService.CreateAsync(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _staffService.GetByIdAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        model.AvailableRoles = await _roleService.GetActiveAsync();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StaffEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await _roleService.GetActiveAsync();

            return View(model);
        }

        await _staffService.UpdateAsync(model);

        return RedirectToAction(nameof(Index));
    }
}