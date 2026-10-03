using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.LabBranches.Commands
{
    public class CreateLabBranchDto
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
   
        public string FullAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string GoogleMapsUrl { get; set; }
        public bool IsPrimary { get; set; } = false;    

    }
}
