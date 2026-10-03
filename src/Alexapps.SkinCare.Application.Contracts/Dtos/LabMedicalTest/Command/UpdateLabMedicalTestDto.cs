using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.LabMedicalTest.Command
{
    public class UpdateLabMedicalTestDto
    {
        public Guid Id { get; set; }
        public Guid TestCategoryId { get; set; }
        public string TestCode { get; set; }
        public double LabPrice { get; set; }
        public string Duration { get; set; }
        public bool IsEnabled { get; set; }

        public LabServiceScope ServiceScope { get; set; }
    }
}
