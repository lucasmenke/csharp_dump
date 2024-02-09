using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared;

public class UserChangePassword
{
    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

}
