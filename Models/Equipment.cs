using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class Equipment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Equipment name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [Range(0, 10000, ErrorMessage = "Quantity cannot be negative.")]
    public int Quantity { get; set; } = 1;

    [StringLength(1000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}