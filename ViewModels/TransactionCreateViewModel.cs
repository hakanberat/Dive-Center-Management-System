using System.ComponentModel.DataAnnotations;
using DiveCenterManager.Models;

namespace DiveCenterManager.ViewModels;

public class TransactionCreateViewModel
{
    [Required(ErrorMessage = "Transaction date is required.")]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
    public int CategoryId { get; set; }

    [Range(
        0.01,
        9999999999,
        ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Currency is required.")]
    public int CurrencyId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public IEnumerable<TransactionCategory> AvailableCategories { get; set; }
        = [];

    public IEnumerable<Currency> AvailableCurrencies { get; set; }
        = [];
}