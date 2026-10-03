using System;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.MedicalTests.Results
{
    public class MedicalTestDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid TestCategoryId { get; set; }
    }
}
