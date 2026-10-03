using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Interfaces
{
    public class NearbyLabRequestDto
    {
        public double CustomerLat { get; set; }
        public double CustomerLong { get; set; }
        public bool IsHomeService { get; set; } // True = سحب عينة من المنزل، False = زيارة معمل
    }
}
