using DiveCenterManager.Models;
using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]

public class RolesController : Controller
{
    private readonly OperationalRoleService _roleService;

    public RolesController(OperationalRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        var roles = await _roleService.GetAllAsync(searchTerm);

        ViewBag.SearchTerm = searchTerm;

        return View(roles);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new OperationalRole());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OperationalRole role)
    {
        if (!ModelState.IsValid)
        {
            return View(role);
        }

        await _roleService.CreateAsync(role);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role is null)
        {
            return NotFound();
        }

        return View(role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(OperationalRole role)
    {
        if (!ModelState.IsValid)
        {
            return View(role);
        }

        await _roleService.UpdateAsync(role);

        return RedirectToAction(nameof(Index));
    }

    
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ToggleActiveStatus(int id)
{
    await _roleService.ToggleActiveStatusAsync(id);

    return RedirectToAction(nameof(Index));
}
}