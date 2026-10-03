using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexapps.SkinCare.Dtos.DiagnosticSessions;
namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class AddIndividualTreatmentPlanDto
    {
        public Guid DiagnosticSessionId { get; set; }
        public Guid MedicationId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int Quantity { get; set; }
        public int Refills { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
