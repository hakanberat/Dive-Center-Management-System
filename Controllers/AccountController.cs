using System.Security.Claims;
using DiveCenterManager.Services;
using DiveCenterManager.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiveCenterManager.Controllers;

public class AccountController : Controller
{
    private readonly AppUserService _appUserService;

    public AccountController(
        AppUserService appUserService)
    {
        _appUserService = appUserService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction(
                "Index",
                "Dashboard");
        }

        return RedirectToAction(
            "Index",
            "Reservations");
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Setup()
    {
        var anyUsers =
            await _appUserService.AnyUsersAsync();

        if (anyUsers)
        {
            return RedirectToAction(nameof(Login));
        }

        return View(new InitialAdminViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Setup(
        InitialAdminViewModel model)
    {
        var anyUsers =
            await _appUserService.AnyUsersAsync();

        if (anyUsers)
        {
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _appUserService.CreateAsync(
            model.Username,
            model.Password,
            model.DisplayName,
            "Admin");

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user =
            await _appUserService.AuthenticateAsync(
                model.Username,
                model.Password);

        if (user is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid username or password.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Name,
                user.Username),

            new(
                "DisplayName",
                user.DisplayName),

            new(
                ClaimTypes.Role,
                user.SystemRole),

            new(
                "SecurityVersion",
                user.SecurityVersion.ToString())
        };

        var identity =
            new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

        var principal =
            new ClaimsPrincipal(identity);

        var authenticationProperties =
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authenticationProperties);

        if (user.SystemRole == "Admin")
        {
            return RedirectToAction(
                "Index",
                "Dashboard");
        }

        return RedirectToAction(
            "Index",
            "Reservations");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}