using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class CreateDiagnosticSessionTreatmentPlanDto
    {
        public Guid DiagnosticSessionId { get; set; }
        public List<TreatmentPlanItemDto> TreatmentPlans { get; set; }
    }

    public class TreatmentPlanItemDto
    {
        public Guid MedicationId { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int? Quantity { get; set; }
        public int? Refills { get; set; }
        public string SpecialInstructions { get; set; }
    }
}
