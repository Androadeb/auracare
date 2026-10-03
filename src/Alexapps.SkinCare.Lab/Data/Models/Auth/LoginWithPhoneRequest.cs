using System.ComponentModel.DataAnnotations;

namespace starterkit.Data.Models.Auth;

public class LoginWithPhoneRequest
{
  [Required, RegularExpression(@"^\+?(\d{1,3})?[-. ]?\(?\d{3}\)?[-. ]?\d{3}[-. ]?\d{4}$")]
  public string Username { get; set; } = string.Empty;

  public string Role { get; set; } = "LAB";
}