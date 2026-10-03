using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.LabMedicalTest.Result
{
    public class LabMedicalTestDto : EntityDto<Guid>
    {
        public Guid MedicalTestId { get; set; }
        public string TestName { get; set; }       
        public string CategoryName { get; set; }   
        //public string TestCode { get; set; }
        public double LabPrice { get; set; }
        //public double TotalPrice { get; set; }
        public string Duration { get; set; }
        public bool IsEnabled { get; set; }

        public LabServiceScope ServiceScope { get; set; }

        public string ServiceScopeName => ServiceScope.ToString();
    }
}
