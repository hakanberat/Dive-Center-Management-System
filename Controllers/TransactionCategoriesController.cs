using DiveCenterManager.Models;
using DiveCenterManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]
public class TransactionCategoriesController : Controller
{
    private readonly TransactionCategoryService _categoryService;

    public TransactionCategoriesController(
        TransactionCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(string? searchTerm)
    {
        var categories = await _categoryService.GetAllAsync(searchTerm);

        ViewBag.SearchTerm = searchTerm;

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TransactionCategory());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TransactionCategory category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        await _categoryService.CreateAsync(category);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        TransactionCategory category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        await _categoryService.UpdateAsync(category);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActiveStatus(int id)
    {
        await _categoryService.ToggleActiveStatusAsync(id);

        return RedirectToAction(nameof(Index));
    }
}