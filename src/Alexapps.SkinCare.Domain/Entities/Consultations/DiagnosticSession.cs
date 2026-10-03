using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Entities.Consultations
{
    public class DiagnosticSession : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid? DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DiagnosticSessionStatus Status { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string ProductsUsed { get; set; }
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsRated { get; set; } = false;
        public ICollection<DiagnosticSessionRoom> VideoRooms { get; set; }
        public ICollection<DiagnosticSessionImage> Images { get; set; }
        public ICollection<DiagnosticSessionMessage> Messages { get; set; }
        public ICollection<DiagnosticSessionTest> Tests { get; set; }
        public ICollection<DiagnosticSessionTreatmentPlan> TreatmentPlans { get; set; }
    }
}
