using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IDoctorScheduleService : IApplicationService
    {
        
        Task<List<DoctorScheduleDto>> GetMyScheduleAsync();

        Task<DiagnosticSessionRoomDto> BookAppointmentAsync(Guid diagnosticSessionId, DateTime scheduledTime);
        Task UpdateFullScheduleAsync(List<DoctorScheduleDto> input);
        Task<DoctorAvailableSlotsDto> GetAvailableSlotsAsync(Guid doctorId, DateTime date);
        Task<DoctorVideoSessionsDto> GetDoctorVideoSessionsDashboardAsync(GetDoctorVideoSessionsQueryDto input);
    }
}
