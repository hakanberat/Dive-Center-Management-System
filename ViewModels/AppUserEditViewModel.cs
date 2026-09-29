using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.ViewModels;

public class AppUserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(150)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        "^(Admin|User)$",
        ErrorMessage = "System role must be Admin or User.")]
    public string SystemRole { get; set; } = "User";
}