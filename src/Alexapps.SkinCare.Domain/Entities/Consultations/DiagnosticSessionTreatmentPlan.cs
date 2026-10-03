using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Medications;
using Alexapps.SkinCare.Enums;
using System;

namespace Alexapps.SkinCare.Entities.Consultations
{
    public class DiagnosticSessionTreatmentPlan : BaseEntity
    {
        public string OrderNumber { get; set; }
        public Guid DiagnosticSessionId { get; set; }
        public DiagnosticSession DiagnosticSession { get; set; }

        public Guid SenderId { get; set; }

        public Guid MedicationId { get; set; }
        public Medication Medication { get; set; }

        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int? Quantity { get; set; }
        public int? Refills { get; set; }
        public string SpecialInstructions { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DetailedAddress { get; set; }
        public TreatmentPlanStatus Status { get; set; } = TreatmentPlanStatus.Received;
        public bool IsRead { get; set; } = false;
        public bool IsPaid { get; set; } = false;
        public Guid? DiagnosticSessionMessageId { get; set; }
        public DiagnosticSessionMessage DiagnosticSessionMessage { get; set; }
    }
}
