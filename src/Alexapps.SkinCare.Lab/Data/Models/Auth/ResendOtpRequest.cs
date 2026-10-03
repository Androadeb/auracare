using System.ComponentModel.DataAnnotations;

namespace starterkit.Data.Models.Auth;

public class ResendOtpRequest
{
    [Required, Phone] public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "LAB";
}