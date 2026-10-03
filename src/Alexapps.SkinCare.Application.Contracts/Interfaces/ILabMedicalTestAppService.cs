using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Command;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Queries;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Interfaces
{
    public interface ILabMedicalTestAppService
    {
        Task<PagedResultWithMetadata<LabMedicalTestDto>> GetListAsync(GetLabMedicalTestListDto input);

        Task<LabMedicalTestDto> CreateAsync(CreateLabMedicalTestDto command);

        Task<LabMedicalTestDto> UpdateAsync(Guid id,UpdateLabMedicalTestDto command);

        Task DeleteAsync(DeleteLabMedicalTestDto input);
    }
}
