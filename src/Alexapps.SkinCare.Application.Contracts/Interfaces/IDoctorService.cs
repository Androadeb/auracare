using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IDoctorService : IApplicationService
    {
        Task<PagedResultWithMetadata<DoctorDto>> GetListAsync(GetDoctorsInput input);
        Task<DoctorDto> GetAsync(System.Guid id);
        Task<List<SpecialtyDto>> GetSpecialtiesAsync();
        Task<List<DoctorScheduleDto>> GetSchedulesAsync(System.Guid id);
    }
}
