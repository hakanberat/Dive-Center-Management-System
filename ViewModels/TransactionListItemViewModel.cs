namespace DiveCenterManager.ViewModels;

public class TransactionListItemViewModel
{
    public int Id { get; set; }

    public DateTime TransactionDate { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public string? CurrencySymbol { get; set; }

    public string? Description { get; set; }
}