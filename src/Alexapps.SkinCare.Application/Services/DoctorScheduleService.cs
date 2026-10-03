using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services
{
    [Authorize]
    public class DoctorScheduleService : ApplicationService, IDoctorScheduleService
    {
        private readonly IRepository<DoctorSchedule, Guid> _doctorScheduleRepository;
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<DiagnosticSessionRoom, Guid> _sessionRoomRepository;
        private readonly IRepository<DiagnosticSession, Guid> _diagnosticSessionRepository;
        private readonly IDiagnosticSessionRoomAppService _videoRoomAppService;
        private readonly INotificationAppService _notificationAppService;

        public DoctorScheduleService(
            IRepository<DoctorSchedule, Guid> doctorScheduleRepository,
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<DiagnosticSessionRoom, Guid> sessionRoomRepository,
            IRepository<DiagnosticSession, Guid> diagnosticSessionRepository,
            IDiagnosticSessionRoomAppService videoRoomAppService,
            INotificationAppService notificationAppService)
        {
            _doctorScheduleRepository = doctorScheduleRepository;
            _doctorRepository = doctorRepository;
            _sessionRoomRepository = sessionRoomRepository;
            _diagnosticSessionRepository = diagnosticSessionRepository;
            _videoRoomAppService = videoRoomAppService;
            _notificationAppService = notificationAppService;
        }

        public async Task<List<DoctorScheduleDto>> GetMyScheduleAsync()
        {
            if (CurrentUser.Id == null) throw new UnauthorizedAccessException("User is not authenticated.");

            var doctor = await _doctorRepository.FirstOrDefaultAsync(d => d.UserId == CurrentUser.Id);
            if (doctor == null) throw new UserFriendlyException("Doctor profile not found.");

            var existingSchedules = await _doctorScheduleRepository.GetListAsync(s => s.DoctorId == doctor.Id);
            var result = new List<DoctorScheduleDto>();
            var allDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

            foreach (var day in allDays)
            {
                var savedDay = existingSchedules.FirstOrDefault(s => s.DayOfWeek == day);
                result.Add(new DoctorScheduleDto
                {
                    Id = savedDay?.Id ?? Guid.Empty,
                    DayOfWeek = day,
                    StartTime = savedDay?.StartTime ?? new TimeSpan(10, 0, 0),
                    EndTime = savedDay?.EndTime ?? new TimeSpan(15, 0, 0),
                    IsAvailable = savedDay?.IsAvailable ?? false
                });
            }

            return result.OrderBy(x => (int)x.DayOfWeek == 6 ? -1 : (int)x.DayOfWeek).ToList();
        }

        public async Task UpdateFullScheduleAsync(List<DoctorScheduleDto> input)
        {
            if (CurrentUser.Id == null) throw new UnauthorizedAccessException();

            var doctor = await _doctorRepository.FirstOrDefaultAsync(d => d.UserId == CurrentUser.Id);
            if (doctor == null) throw new UserFriendlyException("You are not authorized to update schedules.");

            var existingSchedules = await _doctorScheduleRepository.GetListAsync(s => s.DoctorId == doctor.Id);

            foreach (var item in input)
            {
                var existingRecord = existingSchedules.FirstOrDefault(s => s.DayOfWeek == item.DayOfWeek);
                if (existingRecord != null)
                {
                    existingRecord.StartTime = item.StartTime;
                    existingRecord.EndTime = item.EndTime;
                    existingRecord.IsAvailable = item.IsAvailable;
                    await _doctorScheduleRepository.UpdateAsync(existingRecord);
                }
                else
                {
                    await _doctorScheduleRepository.InsertAsync(new DoctorSchedule
                    {
                        DoctorId = doctor.Id,
                        DayOfWeek = item.DayOfWeek,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        IsAvailable = item.IsAvailable
                    });
                }
            }
        }

        public async Task<DiagnosticSessionRoomDto> BookAppointmentAsync(Guid diagnosticSessionId, DateTime scheduledTime)
        {
            var session = await _diagnosticSessionRepository.GetAsync(diagnosticSessionId);

            // 1. Always work with UTC
            var scheduledTimeUtc = DateTime.SpecifyKind(scheduledTime.ToUniversalTime(), DateTimeKind.Utc);
            var nowUtc = DateTime.UtcNow;

            if (scheduledTimeUtc < nowUtc)
            {
                throw new UserFriendlyException(L["CannotBookInPast"]);
            }

            var hasActiveRoom = await _sessionRoomRepository.AnyAsync(r =>
                r.DiagnosticSessionId == diagnosticSessionId &&
                r.Status != VideoRoomStatus.Cancelled &&
                r.Status != VideoRoomStatus.Ended &&
                r.ScheduledEndTime > nowUtc);

            if (hasActiveRoom)
            {
                throw new UserFriendlyException(L["AlreadyHasActiveAppointment"]);
            }

            var localTimeForSchedule = scheduledTimeUtc.ToLocalTime();

            var schedule = await _doctorScheduleRepository.FirstOrDefaultAsync(s =>
                s.DoctorId == session.DoctorId &&
                s.DayOfWeek == localTimeForSchedule.DayOfWeek &&
                s.IsAvailable);

            if (schedule == null || localTimeForSchedule.TimeOfDay < schedule.StartTime || localTimeForSchedule.TimeOfDay >= schedule.EndTime)
            {
                throw new UserFriendlyException(L["DoctorNotWorkingOrOutsideWorkingHours"]);
            }

            var isAlreadyBooked = await _sessionRoomRepository.AnyAsync(r =>
                r.DiagnosticSession.DoctorId == session.DoctorId &&
                r.ScheduledStartTime == scheduledTimeUtc &&
                r.Status != VideoRoomStatus.Cancelled);

            if (isAlreadyBooked)
            {
                throw new UserFriendlyException(L["SlotAlreadyReserved"]);
            }

            // 3. Create the room in UTC
            var result = await _videoRoomAppService.CreateMeetingRoomAsync(diagnosticSessionId, scheduledTimeUtc);
            await _videoRoomAppService.SendBookingConfirmationAsync(diagnosticSessionId, scheduledTimeUtc);

            // 4. Hangfire scheduling logic... (omitted for brevity, keep your existing logic)
            try { /* Your Hangfire logic here */ }
            catch (Exception ex) { Logger.LogError($"Error scheduling Hangfire jobs: {ex.Message}"); }

            // --- Start Notification Fix ---
            // We need to fetch the doctor entity to get the UserId for the notification
            if (session.DoctorId.HasValue)
            {
                var doctorEntity = await _doctorRepository.GetAsync(session.DoctorId.Value);

                await _notificationAppService.NotifyVideoRoomReadyAsync(
                    doctorEntity.UserId,       // Use the UserId from the doctor entity
                    result.VideoRoomId,
                    result.Token,
                    true
                );
            }
            // --- End Notification Fix ---

            return result;
        }

        public async Task<DoctorAvailableSlotsDto> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
        {
            var nowUtc = DateTime.UtcNow;

            // نعتبر التاريخ القادم هو "اليوم" المطلوب محلياً
            var targetLocalDate = date.Date;

            var schedule = await _doctorScheduleRepository.FirstOrDefaultAsync(s =>
                s.DoctorId == doctorId &&
                s.DayOfWeek == targetLocalDate.DayOfWeek &&
                s.IsAvailable);

            var result = new DoctorAvailableSlotsDto
            {
                // نرسل التاريخ موسوماً كـ UTC لضمان عدم تلاعب الـ Serializer به
                Date = DateTime.SpecifyKind(targetLocalDate, DateTimeKind.Utc)
            };

            if (schedule == null) return result;

            var bookedRooms = await _sessionRoomRepository.GetListAsync(r =>
                r.DiagnosticSession.DoctorId == doctorId &&
                r.Status != VideoRoomStatus.Cancelled);

            var currentSlotTime = schedule.StartTime;

            while (currentSlotTime < schedule.EndTime)
            {
                // تكوين التاريخ المحلي للسلوت ثم تحويله لـ UTC
                var localSlotDateTime = targetLocalDate.Add(currentSlotTime);
                var utcSlotDateTime = DateTime.SpecifyKind(localSlotDateTime.ToUniversalTime(), DateTimeKind.Utc);

                var isBooked = bookedRooms.Any(r => r.ScheduledStartTime == utcSlotDateTime);
                var isPastSlot = utcSlotDateTime < nowUtc;

                result.Slots.Add(new SlotItemDto
                {
                    // التسمية للموبايل (ستظهر حسب التاريخ المحلي)
                    TimeLabel = localSlotDateTime.ToString("hh:mm tt"),
                    // القيمة الحقيقية هي UTC صريح
                    ActualDateTime = utcSlotDateTime,
                    IsAvailable = !isBooked && !isPastSlot
                });

                currentSlotTime = currentSlotTime.Add(TimeSpan.FromHours(1));
            }

            return result;
        }

        public async Task<DoctorVideoSessionsDto> GetDoctorVideoSessionsDashboardAsync(GetDoctorVideoSessionsQueryDto input)
        {
        
            var currentPage = input.Page <= 0 ? 1 : input.Page;
            var limit = input.Limit <= 0 ? 8 : input.Limit;

          
            var doctor = await _doctorRepository.FirstOrDefaultAsync(d => d.UserId == CurrentUser.Id);
            if (doctor == null) throw new UserFriendlyException("Doctor profile not found.");

            var nowUtc = DateTime.UtcNow;

           
            var query = await _sessionRoomRepository.WithDetailsAsync(
                r => r.DiagnosticSession,
                r => r.DiagnosticSession.User
            );

         
            var filteredQuery = query
                .Where(r => r.DiagnosticSession.DoctorId == doctor.Id)
                .Where(r => r.Status == (input.IsCompleted ? VideoRoomStatus.Ended : VideoRoomStatus.AppointmentReserved))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), r =>
                    r.DiagnosticSession.User.Name.Contains(input.Filter));

           
            var totalCount = await AsyncExecuter.CountAsync(filteredQuery);

         
            var rooms = await filteredQuery
                .OrderByDescending(r => r.ScheduledStartTime)
                .Skip((currentPage - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var appointmentDtos = new List<VideoSessionSlotDto>();

            foreach (var room in rooms)
            {
                
                if (room.Status == VideoRoomStatus.AppointmentReserved && nowUtc > room.ScheduledEndTime)
                {
                    room.Status = VideoRoomStatus.Ended;
                }

                var dto = ObjectMapper.Map<DiagnosticSessionRoom, VideoSessionSlotDto>(room);

           
                dto.ScheduledStartTime = DateTime.SpecifyKind(room.ScheduledStartTime, DateTimeKind.Utc);

         
                dto.CanStart = nowUtc >= room.ScheduledStartTime.AddMinutes(-10) &&
                               nowUtc <= room.ScheduledEndTime &&
                               room.Status != VideoRoomStatus.Ended;

                dto.IsCompleted = room.Status == VideoRoomStatus.Ended;

                appointmentDtos.Add(dto);
            }

        
            var startOfTodayUtc = nowUtc.Date;
            var weeklyDataQuery = (await _sessionRoomRepository.GetQueryableAsync())
                .Where(r => r.DiagnosticSession.DoctorId == doctor.Id &&
                            r.ScheduledStartTime >= startOfTodayUtc.AddDays(-7) &&
                            r.Status != VideoRoomStatus.Cancelled);

            var todayCallsCount = await AsyncExecuter.CountAsync(weeklyDataQuery.Where(r => r.ScheduledStartTime >= startOfTodayUtc));
            var weeklySessionsCount = await AsyncExecuter.CountAsync(weeklyDataQuery);

      
            return new DoctorVideoSessionsDto
            {
                TodayCallsCount = todayCallsCount,
                WeeklySessionsCount = weeklySessionsCount,
                AvgDuration = "60m",

                Appointments = new PagedResultWithMetadata<VideoSessionSlotDto>(
                    appointmentDtos, // List<T> items
                    currentPage,     // int page
                    limit,           // int limit
                    totalCount       // long totalCount
                )
            };
        }
    }
}