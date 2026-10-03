using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.MedicalTests.Results
{
    public class SampleTypeDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
