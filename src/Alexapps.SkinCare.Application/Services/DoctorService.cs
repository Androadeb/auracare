using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services
{
    public class DoctorService : SkinCareAppService, IDoctorService
    {
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<Specialty, Guid> _specialtyRepository;
        private readonly IRepository<DoctorSchedule, Guid> _doctorScheduleRepository;

        public DoctorService(
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<Specialty, Guid> specialtyRepository,
            IRepository<DoctorSchedule, Guid> doctorScheduleRepository)
        {
            _doctorRepository = doctorRepository;
            _specialtyRepository = specialtyRepository;
            _doctorScheduleRepository = doctorScheduleRepository;
        }

        public async Task<PagedResultWithMetadata<DoctorDto>> GetListAsync(GetDoctorsInput input)
        {
            var queryable = await _doctorRepository.WithDetailsAsync(x => x.User, x => x.Specialty);
            queryable = queryable.Where(x => x.Type == DoctorTypeEnum.SessionProvider);
            
            // Apply filters
            if (!string.IsNullOrWhiteSpace(input.Name))
            {
                queryable = queryable.Where(x => x.User.Name.Contains(input.Name));
            }
            
            if (input.SpecialtyId.HasValue)
            {
                queryable = queryable.Where(x => x.SpecialtyId == input.SpecialtyId.Value);
            }
            
            if (input.Gender.HasValue)
            {
                var genderString = input.Gender.Value == UserGenderEnum.Male ? "male" : "female";
                queryable = queryable.Where(x => x.User.Gender == genderString);
            }
            
            // Apply sorting
            if (input.SortByTopRated)
            {
                queryable = queryable.OrderByDescending(x => x.Rating);
            }
            else
            {
                queryable = queryable.OrderBy(x => x.User.Name);
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            
            // Calculate skip based on page and limit
            var skipCount = (input.Page - 1) * input.Limit;
            
            var doctors = await AsyncExecuter.ToListAsync(
                queryable.Skip(skipCount).Take(input.Limit)
            );

            var dtos = ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(doctors);
            
            return new PagedResultWithMetadata<DoctorDto>(dtos, input.Page, input.Limit, totalCount);
        }

        public async Task<DoctorDto> GetAsync(Guid id)
        {
            var queryable = await _doctorRepository.WithDetailsAsync(x => x.User, x => x.Specialty);
            var doctor = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));
            
            return ObjectMapper.Map<Doctor, DoctorDto>(doctor);
        }

        public async Task<List<SpecialtyDto>> GetSpecialtiesAsync()
        {
            var specialties = await _specialtyRepository.GetListAsync();
            return ObjectMapper.Map<List<Specialty>, List<SpecialtyDto>>(specialties);
        }

        public async Task<List<DoctorScheduleDto>> GetSchedulesAsync(Guid id)
        {
            var schedules = await _doctorScheduleRepository.GetListAsync(x => x.DoctorId == id && x.IsAvailable);
            return ObjectMapper.Map<List<DoctorSchedule>, List<DoctorScheduleDto>>(schedules);
        }
    }
}
