using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Doctors;
using System;

namespace Alexapps.SkinCare.Entities.HomeServices
{
    public class HomeServiceProvider : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public Guid HomeServiceId { get; set; }
        public HomeService HomeService { get; set; }
    }
}
