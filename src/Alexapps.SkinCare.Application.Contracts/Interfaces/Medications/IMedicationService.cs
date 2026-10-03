using Alexapps.SkinCare.Dtos.Medications.Commands;
using Alexapps.SkinCare.Dtos.Medications.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces.Medications
{
    public interface IMedicationService : IApplicationService
    {
        Task<MedicationDto> CreateAsync(CreateUpdateMedicationDto input);
        Task<MedicationDto> UpdateAsync(Guid id, CreateUpdateMedicationDto input);
        Task DeleteAsync(Guid id);
        Task<PagedResultDto<MedicationDto>> GetListAsync(string keyword, PagedAndSortedResultRequestDto input);
        Task<List<MedicationDto>> GetAllAsync();
        Task<MedicationDto> GetAsync(Guid id);
    }
}
