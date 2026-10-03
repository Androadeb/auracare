using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.LabBranches.Commands
{
    public class UpdateLabBranchDto : CreateLabBranchDto
    {
       
        public bool IsActive { get; set; }
       
    }
}
