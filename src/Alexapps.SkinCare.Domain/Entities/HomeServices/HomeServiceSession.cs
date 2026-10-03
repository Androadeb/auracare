using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alexapps.SkinCare.Entities.HomeServices
{
    public class HomeServiceSession : BaseEntity
    {
        public Guid HomeServiceId { get; set; }
        public HomeService HomeService { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime SessionDate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string? PhoneNumber { get; set; }

        public HomeServiceSessionStatus Status { get; set; }
    }
}
