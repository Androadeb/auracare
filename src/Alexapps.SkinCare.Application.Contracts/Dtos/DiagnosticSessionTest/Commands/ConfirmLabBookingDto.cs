using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Commands
{
    public class ConfirmLabBookingDto
    {
       
        [Required]
        public Guid BranchId { get; set; } 

        
        [Required]
        public DateTime AppointmentDate { get; set; }

       
        [Required]
        public LabServiceType ServiceType { get; set; }

     
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DetailedAddress { get; set; }
    }
}
