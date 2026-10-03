using Alexapps.SkinCare.Entities.LABs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Seeders
{
    public class LabSchedulesDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Lab, Guid> _labRepository;
        private readonly IRepository<LabSchedule, Guid> _labScheduleRepository;

        public LabSchedulesDataSeeder(
            IRepository<Lab, Guid> labRepository,
            IRepository<LabSchedule, Guid> labScheduleRepository)
        {
            _labRepository = labRepository;
            _labScheduleRepository = labScheduleRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // 1. جلب كل المعامل الموجودة في النظام
            var labs = await _labRepository.GetListAsync();

            foreach (var lab in labs)
            {
                // 2. التحقق من وجود الـ 7 أيام لكل معمل
                var existingSchedulesCount = await _labScheduleRepository.CountAsync(x => x.LabId == lab.Id);

                if (existingSchedulesCount < 7)
                {
                    var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var day in daysOfWeek)
                    {
                        // 3. التأكد من أن هذا اليوم بالتحديد غير موجود للمعمل الحالي
                        var alreadyExists = await _labScheduleRepository.AnyAsync(x =>
                            x.LabId == lab.Id && x.DayOfWeek == day);

                        if (!alreadyExists)
                        {
                            await _labScheduleRepository.InsertAsync(new LabSchedule
                            {
                                LabId = lab.Id,
                                DayOfWeek = day,
                               
                                OpeningTime = new TimeSpan(10, 0, 0), 
                                ClosingTime = new TimeSpan(15, 0, 0), 
                                IsOpen = false,
                                CapacityPerHour = 4
                            }, autoSave: true);
                        }
                    }
                }
            }
        }
    }
}