using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers
{
    public class DoctorScheduleController : AbpController
    {
        private readonly IDoctorScheduleService _scheduleService;

        public DoctorScheduleController(IDoctorScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        [Route("api/v1/doctor/my-schedules")]
        [ApiExplorerSettings(GroupName = "doctor")]
        public async Task<List<DoctorScheduleDto>> GetMyScheduleAsync()
        {
            return await _scheduleService.GetMyScheduleAsync();
        }

        [HttpPut]
        [Route("api/v1/doctor/my-schedules")]
        [ApiExplorerSettings(GroupName = "doctor")]
        public async Task UpdateFullScheduleAsync([FromBody] List<DoctorScheduleDto> input)
        {
         
            await _scheduleService.UpdateFullScheduleAsync(input);
        }

        [HttpGet]
        [Route("api/v1/doctor/video-sessions-dashboard")]
        [ApiExplorerSettings(GroupName = "doctor")]
        public async Task<DoctorVideoSessionsDto> GetDoctorVideoSessionsDashboardAsync([FromQuery] GetDoctorVideoSessionsQueryDto input)
        {
           
            return await _scheduleService.GetDoctorVideoSessionsDashboardAsync(input);
        }

        [HttpGet]
     
        [Route("api/v1/doctors/{id}/available-schedules")]
        [ApiExplorerSettings(GroupName = "client")]
        public virtual Task<DoctorAvailableSlotsDto> GetAvailableSlotsAsync(Guid id, DateTime date)
        {
            return _scheduleService.GetAvailableSlotsAsync(id, date);
        }

        [HttpPost]
        [Route("api/v1/diagnostic-sessions/book-appointment")]
        [ApiExplorerSettings(GroupName = "client")]
        public virtual Task<DiagnosticSessionRoomDto> BookAppointmentAsync(Guid diagnosticSessionId, DateTime scheduledTime)
        {
            return _scheduleService.BookAppointmentAsync(diagnosticSessionId, scheduledTime);
        }
    }
}