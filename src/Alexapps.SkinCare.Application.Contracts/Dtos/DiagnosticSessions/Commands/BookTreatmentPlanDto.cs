using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class BookTreatmentPlanDto
    {

        public Guid DiagnosticSessionMessageId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DetailedAddress { get; set; }
    }
}
