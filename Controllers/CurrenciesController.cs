using DiveCenterManager.Models;
using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class CurrenciesController : Controller
{
    private readonly CurrencyService _currencyService;

    public CurrenciesController(CurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        var currencies = await _currencyService.GetAllAsync(searchTerm);

        ViewBag.SearchTerm = searchTerm;

        return View(currencies);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Currency());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Currency currency)
    {
        if (!ModelState.IsValid)
        {
            return View(currency);
        }

        await _currencyService.CreateAsync(currency);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var currency = await _currencyService.GetByIdAsync(id);

        if (currency is null)
        {
            return NotFound();
        }

        return View(currency);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Currency currency)
    {
        if (!ModelState.IsValid)
        {
            return View(currency);
        }

        await _currencyService.UpdateAsync(currency);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActiveStatus(int id)
    {
        await _currencyService.ToggleActiveStatusAsync(id);

        return RedirectToAction(nameof(Index));
    }
}