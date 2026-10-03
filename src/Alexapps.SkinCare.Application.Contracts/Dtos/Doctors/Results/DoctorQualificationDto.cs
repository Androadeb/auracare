using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class DoctorQualificationDto
    {
        public Guid Id { get; set; }
        public string Degree { get; set; }
        public string CertificateImageUrl { get; set; }
    }
}
