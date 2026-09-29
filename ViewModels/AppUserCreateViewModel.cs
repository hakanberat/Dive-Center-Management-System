using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.ViewModels;

public class AppUserCreateViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(150)]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password confirmation is required.")]
    [DataType(DataType.Password)]
    [Compare(
        nameof(Password),
        ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        "^(Admin|User)$",
        ErrorMessage = "System role must be Admin or User.")]
    public string SystemRole { get; set; } = "User";
}