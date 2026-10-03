using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IQualificationAppService : IApplicationService
    {
        Task<DoctorQualificationDto> CreateAsync(UpdateQualificationDto input);
        Task<DoctorQualificationDto> UpdateAsync(Guid id, UpdateQualificationDto input);
        Task<List<DoctorQualificationDto>> GetMyQualificationsAsync();
        Task DeleteAsync(Guid id);
    }
}
