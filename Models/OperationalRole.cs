using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class OperationalRole
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Role name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}