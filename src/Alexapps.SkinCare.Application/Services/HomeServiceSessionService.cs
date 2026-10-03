using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Commands;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Results;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Queries;
using Alexapps.SkinCare.Entities.HomeServices;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Microsoft.EntityFrameworkCore;
using Alexapps.SkinCare.Dtos.Common;

namespace Alexapps.SkinCare.Services
{
    [Authorize]
    public class HomeServiceSessionService : SkinCareAppService, IHomeServiceSessionService
    {
        private readonly IRepository<HomeServiceSession, Guid> _homeServiceSessionRepository;
        private readonly IRepository<HomeServiceSchedule, Guid> _homeServiceScheduleRepository;

        public HomeServiceSessionService(
            IRepository<HomeServiceSession, Guid> homeServiceSessionRepository,
            IRepository<HomeServiceSchedule, Guid> homeServiceScheduleRepository)
        {
            _homeServiceSessionRepository = homeServiceSessionRepository;
            _homeServiceScheduleRepository = homeServiceScheduleRepository;
        }

        public async Task<HomeServiceSessionDto> CreateAsync(CreateHomeServiceSessionDto input)
        {
            var session = new HomeServiceSession
            {
                HomeServiceId = input.HomeServiceId,
                DoctorId = input.DoctorId,
                CustomerId = CurrentUser.Id.GetValueOrDefault(),
                SessionDate = input.SessionDate,
                Latitude = input.Latitude,
                Longitude = input.Longitude,
                Address = input.Address,
                PhoneNumber = input.PhoneNumber,
                Status = HomeServiceSessionStatus.Pending
            };

            await _homeServiceSessionRepository.InsertAsync(session, autoSave: true);

            return ObjectMapper.Map<HomeServiceSession, HomeServiceSessionDto>(session);
        }

        public async Task<HomeServiceSessionDto> GetAsync(Guid id)
        {
            var session = await _homeServiceSessionRepository.GetAsync(id);
            return ObjectMapper.Map<HomeServiceSession, HomeServiceSessionDto>(session);
        }

        public async Task<List<HomeServiceScheduleDto>> GetSchedulesAsync(Guid homeServiceId)
        {
            var schedules = await _homeServiceScheduleRepository.GetListAsync(x => x.HomeServiceId == homeServiceId && x.IsAvailable);
            return ObjectMapper.Map<List<HomeServiceSchedule>, List<HomeServiceScheduleDto>>(schedules);
        }

        public async Task<PagedResultWithMetadata<HomeServiceSessionDto>> GetListAsync(GetHomeServiceSessionsInput input)
        {
            var query = (await _homeServiceSessionRepository.GetQueryableAsync())
                .Include(x => x.HomeService)
                .Include(x => x.Doctor).ThenInclude(x => x.User)
                .Include(x => x.Doctor).ThenInclude(x => x.Specialty);

            var userId = CurrentUser.Id.GetValueOrDefault();

            var filteredQuery = query
                .Where(x => x.CustomerId == userId || x.Doctor.UserId == userId)
                .WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value)
                .WhereIf(input.DoctorId.HasValue, x => x.DoctorId == input.DoctorId.Value)
                .WhereIf(input.HomeServiceId.HasValue, x => x.HomeServiceId == input.HomeServiceId.Value);

            var totalCount = await filteredQuery.LongCountAsync();

            var items = await filteredQuery
                .OrderByDescending(x => x.CreationTime)
                .Skip((Math.Max(1, input.Page) - 1) * input.Limit)
                .Take(input.Limit)
                .ToListAsync();

            var dtos = ObjectMapper.Map<List<HomeServiceSession>, List<HomeServiceSessionDto>>(items);

            return new PagedResultWithMetadata<HomeServiceSessionDto>(dtos, input.Page, input.Limit, totalCount);
        }
    }
}
