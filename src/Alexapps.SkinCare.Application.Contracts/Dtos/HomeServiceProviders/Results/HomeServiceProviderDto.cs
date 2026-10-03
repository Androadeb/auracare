using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using System;

namespace Alexapps.SkinCare.Dtos.HomeServiceProviders.Results
{
    public class HomeServiceProviderDto
    {
        public Guid Id { get; set; }
        public DoctorDto Doctor { get; set; }
    }
}
