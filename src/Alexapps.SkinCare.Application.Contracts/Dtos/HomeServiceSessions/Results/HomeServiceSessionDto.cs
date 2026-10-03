using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using Alexapps.SkinCare.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.HomeServiceSessions.Results
{
    public class HomeServiceSessionDto : EntityDto<Guid>
    {
        public Guid HomeServiceId { get; set; }
        public HomeServiceDto HomeService { get; set; }

        public Guid DoctorId { get; set; }
        public DoctorDto Doctor { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime SessionDate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string? PhoneNumber { get; set; }

        public HomeServiceSessionStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
