using Alexapps.SkinCare.Dtos.LabSchedule.Command;
using Alexapps.SkinCare.Dtos.LabSchedule.Queries;
using Alexapps.SkinCare.Dtos.LabSchedule.Result;
using Alexapps.SkinCare.Entities.LABs; // Ensure this is your correct entity namespace
using Alexapps.SkinCare.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services.LabSchedules
{
    [Authorize]
    public class LabScheduleAppService : ApplicationService, ILabScheduleAppService
    {
        private readonly IRepository<LabSchedule, Guid> _labScheduleRepository;
        private readonly IRepository<Lab, Guid> _labRepository;

        public LabScheduleAppService(
            IRepository<LabSchedule, Guid> labScheduleRepository,
            IRepository<Lab, Guid> labRepository)
        {
            _labScheduleRepository = labScheduleRepository;
            _labRepository = labRepository;
        }

       
        public async Task<List<LabScheduleResult>> GetListAsync()
        {
            var labId = await GetCurrentLabIdAsync();

        
            var schedules = await _labScheduleRepository.GetListAsync(x => x.LabId == labId);

         
            return ObjectMapper.Map<List<LabSchedule>, List<LabScheduleResult>>(
                schedules.OrderBy(x => x.DayOfWeek).ToList()
            );
        }


        public async Task<List<LabScheduleResult>> UpdateAllAsync(UpdateAllLabSchedulesDto input)
        {
            var labId = await GetCurrentLabIdAsync();

            foreach (var item in input.Schedules)
            {
                var schedule = await _labScheduleRepository.FirstOrDefaultAsync(x =>
                    x.Id == item.Id && x.LabId == labId);

                if (schedule != null)
                {
                    schedule.OpeningTime = item.OpeningTime;
                    schedule.ClosingTime = item.ClosingTime;
                    schedule.IsOpen = item.IsOpen;
                    schedule.CapacityPerHour = item.CapacityPerHour;

                    await _labScheduleRepository.UpdateAsync(schedule);
                }
            }

            // بعد ما نخلص التحديث، نرجع القائمة الجديدة كاملة
            return await GetListAsync();
        }

        private async Task<Guid> GetCurrentLabIdAsync()
        {
            var userId = CurrentUser.Id.GetValueOrDefault();
            var lab = await _labRepository.FirstOrDefaultAsync(l => l.UserId == userId);
            if (lab == null) throw new UserFriendlyException("Lab not found.");
            return lab.Id;
        }
    }
}