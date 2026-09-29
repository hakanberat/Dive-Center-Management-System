using DiveCenterManager.Services;
using DiveCenterManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly AppUserService _appUserService;

    public UsersController(
        AppUserService appUserService)
    {
        _appUserService = appUserService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users =
            await _appUserService.GetAllAsync();

        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AppUserCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AppUserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usernameExists =
            await _appUserService.UsernameExistsAsync(
                model.Username);

        if (usernameExists)
        {
            ModelState.AddModelError(
                nameof(model.Username),
                "This username is already in use.");

            return View(model);
        }

        await _appUserService.CreateAsync(
            model.Username,
            model.Password,
            model.DisplayName,
            model.SystemRole);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user =
            await _appUserService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        AppUserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usernameExists =
            await _appUserService.UsernameExistsAsync(
                model.Username,
                model.Id);

        if (usernameExists)
        {
            ModelState.AddModelError(
                nameof(model.Username),
                "This username is already in use.");

            return View(model);
        }

        var existingUser =
            await _appUserService
                .GetAuthenticationUserByIdAsync(model.Id);

        if (existingUser is null)
        {
            return NotFound();
        }

        if (existingUser.SystemRole == "Admin" &&
            existingUser.IsActive &&
            model.SystemRole != "Admin")
        {
            var activeAdminCount =
                await _appUserService
                    .GetActiveAdminCountAsync();

            if (activeAdminCount <= 1)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The last active Admin cannot be changed to User.");

                return View(model);
            }
        }

        await _appUserService.UpdateAsync(model);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var model =
            await _appUserService.GetResetPasswordAsync(id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        AppUserResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var success =
            await _appUserService.ResetPasswordAsync(
                model.Id,
                model.NewPassword);

        if (!success)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var user =
            await _appUserService
                .GetAuthenticationUserByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        if (user.SystemRole == "Admin" &&
            user.IsActive)
        {
            var activeAdminCount =
                await _appUserService
                    .GetActiveAdminCountAsync();

            if (activeAdminCount <= 1)
            {
                TempData["ErrorMessage"] =
                    "The last active Admin cannot be deactivated.";

                return RedirectToAction(nameof(Index));
            }
        }

        await _appUserService.ToggleActiveStatusAsync(id);

        return RedirectToAction(nameof(Index));
    }
}