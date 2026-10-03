using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.LabBranches.Queries
{
    public class GetLabBranchesInput
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Filter { get; set; }
    }
}
