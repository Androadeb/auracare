using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class DoctorRatingDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public string PatientName { get; set; } // From User.Name
        public string PatientPictureUrl { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
