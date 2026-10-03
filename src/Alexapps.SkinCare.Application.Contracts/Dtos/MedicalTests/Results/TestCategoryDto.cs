using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.MedicalTests.Results
{
    public class TestCategoryDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
