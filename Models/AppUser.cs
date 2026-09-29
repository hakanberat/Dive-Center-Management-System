using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        "^(Admin|User)$",
        ErrorMessage = "System role must be Admin or User.")]
    public string SystemRole { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    public int SecurityVersion { get; set; } = 1;

    public DateTime CreatedAt { get; set; }
}