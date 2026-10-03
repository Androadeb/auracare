using Alexapps.SkinCare.Entities.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Seeders
{
    public class DoctorSchedulesDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<DoctorSchedule, Guid> _doctorScheduleRepository;

        public DoctorSchedulesDataSeeder(
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<DoctorSchedule, Guid> doctorScheduleRepository)
        {
            _doctorRepository = doctorRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
          
            var doctors = await _doctorRepository.GetListAsync();

            foreach (var doctor in doctors)
            {
            
                var existingSchedulesCount = await _doctorScheduleRepository.CountAsync(x => x.DoctorId == doctor.Id);

                if (existingSchedulesCount < 7)
                {
                    var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

                    foreach (var day in daysOfWeek)
                    {
                     
                        var alreadyExists = await _doctorScheduleRepository.AnyAsync(x =>
                            x.DoctorId == doctor.Id && x.DayOfWeek == day);

                        if (!alreadyExists)
                        {
                            await _doctorScheduleRepository.InsertAsync(new DoctorSchedule
                            {
                                DoctorId = doctor.Id,
                                DayOfWeek = day,
                              
                                StartTime = new TimeSpan(10, 0, 0),
                                EndTime = new TimeSpan(15, 0, 0),
                                IsAvailable = false
                            }, autoSave: true);
                        }
                    }
                }
            }
        }
    }
    }
