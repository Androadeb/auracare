using Alexapps.SkinCare.Enums;
using Microsoft.AspNetCore.Http;

namespace Alexapps.SkinCare.Dtos.Auth.Commands.Register;

public class RegisterCommand
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public UserGenderEnum UserGender { get; set; }
    public IFormFile ProfileImage { get; set; }
}

