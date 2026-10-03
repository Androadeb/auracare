using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.Doctors
{
    public class DoctorRating : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid DiagnosticSessionId { get; set; } 

        public int Stars { get; set; } 
        public string Comment { get; set; }
    }
}
