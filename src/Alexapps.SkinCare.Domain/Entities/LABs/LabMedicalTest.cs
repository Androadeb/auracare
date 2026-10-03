using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.MedicalTests;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.LABs
{
    public class LabMedicalTest : BaseEntity
    {
        public Guid LabId { get; set; }
        public Lab Lab { get; set; } 

        public Guid MedicalTestId { get; set; }
        public MedicalTest MedicalTest { get; set; }

        public string TestCode { get; set; }
        public double LabPrice { get; set; }
        public double? TotalPrice { get; set; }
        public string Duration { get; set; }
        public bool IsEnabled { get; set; }
        public LabServiceScope ServiceScope { get; set; }

    }
}
