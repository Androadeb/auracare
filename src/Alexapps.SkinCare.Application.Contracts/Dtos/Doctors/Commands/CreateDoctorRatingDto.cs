using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Commands
{
    public class CreateDoctorRatingDto
    {
        public Guid DiagnosticSessionId { get; set; }

        public int Stars { get; set; } // 1 to 5

        public string Comment { get; set; }
    }
}
