using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;


namespace Alexapps.SkinCare.Dtos.LabMedicalTest.Queries
{
    public class GetLabMedicalTestListDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Filter { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
