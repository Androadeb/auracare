using Alexapps.SkinCare.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.LABs
{
    public class LabBranch : BaseEntity
    {
        public Guid LabId { get; set; } 
        public Lab Lab { get; set; }

        public string Name { get; set; } 
        public string ContactNumber { get; set; } 

        public string FullAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string GoogleMapsUrl { get; set; }

        public bool IsPrimary { get; set; } 
        public bool IsActive { get; set; }
    }
}
