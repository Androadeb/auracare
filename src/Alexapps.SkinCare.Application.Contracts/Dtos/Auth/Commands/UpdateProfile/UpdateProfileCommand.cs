using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Alexapps.SkinCare.Dtos.Auth.Commands.UpdateProfile;

public class UpdateProfileCommand
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string PhoneNumber { get; set; }

    public string? Email { get; set; }
    public UserGenderEnum? UserGender { get; set; }
    public IFormFile? ProfileImage { get; set; }
    public bool DeleteProfileImage { get; set; }

   
    public Guid? SpecialtyId { get; set; }
    public int? YearsOfExperience { get; set; }
    public List<UpdateQualificationDto>? Qualifications { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }

    public bool? IsActive { get; set; }
}
