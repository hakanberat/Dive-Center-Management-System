using DiveCenterManager.Services;
using DiveCenterManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace DiveCenterManager.Controllers;

[Authorize(Roles = "Admin")]

public class TransactionsController : Controller
{
    private readonly TransactionService _transactionService;
    private readonly TransactionCategoryService _categoryService;
    private readonly CurrencyService _currencyService;

    public TransactionsController(
        TransactionService transactionService,
        TransactionCategoryService categoryService,
        CurrencyService currencyService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
        _currencyService = currencyService;
    }

    public async Task<IActionResult> Index()
    {
        var transactions = await _transactionService.GetAllAsync();

        return View(transactions);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new TransactionCreateViewModel
        {
            AvailableCategories = await _categoryService.GetActiveAsync(),
            AvailableCurrencies = await _currencyService.GetActiveAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TransactionCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableCategories =
                await _categoryService.GetActiveAsync();

            model.AvailableCurrencies =
                await _currencyService.GetActiveAsync();

            return View(model);
        }

        await _transactionService.CreateAsync(model);

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
public async Task<IActionResult> Edit(int id)
{
    var model = await _transactionService.GetByIdAsync(id);

    if (model is null)
    {
        return NotFound();
    }

    model.AvailableCategories =
        await _categoryService.GetActiveAsync();

    model.AvailableCurrencies =
        await _currencyService.GetActiveAsync();

    ViewBag.TransactionId = id;

    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(
    int id,
    TransactionCreateViewModel model)
{
    if (!ModelState.IsValid)
    {
        model.AvailableCategories =
            await _categoryService.GetActiveAsync();

        model.AvailableCurrencies =
            await _currencyService.GetActiveAsync();

        ViewBag.TransactionId = id;

        return View(model);
    }

    await _transactionService.UpdateAsync(id, model);

    return RedirectToAction(nameof(Index));
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    await _transactionService.DeleteAsync(id);

    return RedirectToAction(nameof(Index));
}

}