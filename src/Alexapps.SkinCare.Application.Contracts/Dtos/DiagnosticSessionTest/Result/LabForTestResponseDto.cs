using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result
{
    public class LabForTestResponseDto
    {
        public Guid LabId { get; set; } 
        public Guid BranchId { get; set; } 
        public string LabName { get; set; } 
        public string BranchName { get; set; } 
        public string FullAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double LabPrice { get; set; }
        public double Distance { get; set; }
        public bool OffersHomeService { get; set; }
        public bool IsAvailableInSelectedTime { get; set; }
    }
}
