using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries
{
    public class GetAvailableTimeSlotsInput
    {
        [Required]
        public Guid BranchId { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
