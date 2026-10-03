namespace starterkit.Data.Models.Auth;

public class OtpRequest
{
  public string Username { get; set; } = string.Empty;
  public string Otp { get; set; } = string.Empty;
}