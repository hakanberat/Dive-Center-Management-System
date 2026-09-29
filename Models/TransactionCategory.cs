using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class TransactionCategory
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Transaction type is required.")]
    [RegularExpression(
        "^(Income|Expense)$",
        ErrorMessage = "Type must be Income or Expense.")]
    public string Type { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}