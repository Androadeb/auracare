using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.LabBranches.Result;
using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Business.Client.Profile.Results;

public class GetProfileResult
{

    public string Name { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Gender { get; set; }
    public string Role { get; set; }

  
    public Guid? SpecialtyId { get; set; }
    public string? SpecialtyNameAr { get; set; }

    public string? SpecialtyNameEn { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public List<DoctorQualificationDto>? Qualifications { get; set; }
    public bool CanPublishBlogs { get; set; }
    public bool IsNotificationsEnabled { get; set; }

    public int UnreadNotificationsCount { get; set; }
    public bool? IsActive { get; set; }
    public List<LabBranchDto>? Branches { get; set; }
}

