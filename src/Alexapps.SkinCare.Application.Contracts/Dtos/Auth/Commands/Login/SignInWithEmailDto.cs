using System.ComponentModel.DataAnnotations;

namespace Alexapps.SkinCare.Dtos.Auth.Commands.Login;

public class SignInWithEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
