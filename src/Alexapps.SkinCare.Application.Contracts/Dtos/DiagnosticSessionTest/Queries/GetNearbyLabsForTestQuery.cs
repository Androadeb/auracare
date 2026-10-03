using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries
{
    public class GetNearbyLabsForTestQuery
    {
        [Required]
        public Guid DiagnosticSessionTestId { get; set; }

        [Required]
        public double CustomerLat { get; set; }

        [Required]
        public double CustomerLong { get; set; }

    
        public bool IsHomeService { get; set; }

       
    }
}
