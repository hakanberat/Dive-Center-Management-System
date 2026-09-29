using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class Boat
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Boat name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? RegistrationNumber { get; set; }

    [Range(1, 500, ErrorMessage = "Capacity must be greater than zero.")]
    public int Capacity { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
}