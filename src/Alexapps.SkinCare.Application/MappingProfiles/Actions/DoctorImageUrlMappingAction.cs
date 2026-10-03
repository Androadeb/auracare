using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Entities.Doctors;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions;

public class DoctorImageUrlMappingAction : IMappingAction<Doctor, DoctorDto>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public DoctorImageUrlMappingAction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Process(Doctor source, DoctorDto destination, ResolutionContext context)
    {
        var baseUrl = _configuration["App:SelfUrl"];
        if (string.IsNullOrEmpty(baseUrl)) return;

        baseUrl = baseUrl.TrimEnd('/');

       
        if (!string.IsNullOrEmpty(source.User?.ProfileImage))
        {
            var image = source.User.ProfileImage.TrimStart('/');
            destination.Image = $"{baseUrl}/{image}";
        }

        if (source.Qualifications != null && destination.Qualifications != null)
        {
            foreach (var qualDto in destination.Qualifications) 
            {
                if (!string.IsNullOrEmpty(qualDto.CertificateImageUrl) &&
                    !qualDto.CertificateImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var certPath = qualDto.CertificateImageUrl.TrimStart('/');
                    qualDto.CertificateImageUrl = $"{baseUrl}/{certPath}";
                }
            }
        }
    }
}
