using System.ComponentModel.DataAnnotations;
using DiveCenterManager.Models;

namespace DiveCenterManager.ViewModels;

public class StaffEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? CertificationAgency { get; set; }

    [StringLength(100)]
    public string? InstructorNumber { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public List<int> SelectedRoleIds { get; set; } = [];

    public IEnumerable<OperationalRole> AvailableRoles { get; set; } = [];
}