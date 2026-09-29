using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class DiveActivity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Activity name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 9999999, ErrorMessage = "Price cannot be negative.")]
    public decimal DefaultPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Currency is required.")]
    public int CurrencyId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public string CurrencyCode { get; set; } = string.Empty;

    public string? CurrencySymbol { get; set; }
}