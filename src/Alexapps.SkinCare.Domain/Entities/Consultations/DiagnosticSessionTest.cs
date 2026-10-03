using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.MedicalTests;
using Alexapps.SkinCare.Enums;
using System;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.Consultations
{
    public class DiagnosticSessionTest : BaseEntity
    {
        public Guid DiagnosticSessionId { get; set; }
        public DiagnosticSession DiagnosticSession { get; set; }
        public Guid? BranchId { get; set; }
        public LabBranch Branch { get; set; }
        public Guid SenderId { get; set; }
        
        public Guid MedicalTestId { get; set; }
        public MedicalTest MedicalTest { get; set; }
        
        public Guid SampleTypeId { get; set; }
        public SampleType SampleType { get; set; }

        public Guid? LabId { get; set; }
        public Lab Lab { get; set; }

        public long OrderNumber { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool IsPaid { get; set; } = false;
    
        public DateTime? AppointmentDate { get; set; }

        public DiagnosticTestStatus Status { get; set; } = DiagnosticTestStatus.Requested;

       
        public LabServiceType ServiceType { get; set; }
        public string? ResultFileName { get; set; }
        public string? ResultFileUrl { get; set; }

        public double? Latitude { get; set; } 
        public double? Longitude { get; set; }
        public string? DetailedAddress { get; set; } 
        public string? PreparationInstructions { get; set; }

        public bool IsRead { get; set; } = false;
        public string BookingCode { get; set; }
        public void GenerateBookingCode()
        {
            if (string.IsNullOrEmpty(BookingCode))
            {
                BookingCode = "LAB-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            }
        }

    }

}
