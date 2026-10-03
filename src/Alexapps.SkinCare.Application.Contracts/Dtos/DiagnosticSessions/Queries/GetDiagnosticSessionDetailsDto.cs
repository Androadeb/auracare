using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries
{
    public class GetDiagnosticSessionDetailsDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Filter { get; set; }

  
        public bool IncludeTests { get; set; } = true;

        
        public bool IncludeTreatmentPlans { get; set; } = true;

        
        public bool IncludeSessionFiles { get; set; } = true;
    }
}
