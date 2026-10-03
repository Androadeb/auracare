using Alexapps.SkinCare.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.Doctors
{
    public class DoctorQualification : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public string Degree { get; set; }
        public string CertificateImageUrl { get; set; }
    }
}
