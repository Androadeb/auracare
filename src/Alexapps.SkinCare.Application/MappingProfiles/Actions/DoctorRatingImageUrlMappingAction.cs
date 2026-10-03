using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Entities.Doctors;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions
{
    public class DoctorRatingImageUrlMappingAction : IMappingAction<DoctorRating, DoctorRatingDto>, ITransientDependency
    {
        private readonly IConfiguration _configuration;

        public DoctorRatingImageUrlMappingAction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Process(DoctorRating source, DoctorRatingDto destination, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(destination.PatientPictureUrl))
            {
                var baseUrl = _configuration["App:SelfUrl"];

                if (!string.IsNullOrEmpty(baseUrl))
                {
                   
                    if (!destination.PatientPictureUrl.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var image = destination.PatientPictureUrl.TrimStart('/');
                        destination.PatientPictureUrl = $"{baseUrl.TrimEnd('/')}/{image}";
                    }
                }
            }
        }
    }
}
