using Alexapps.SkinCare.Dtos.HomeServiceSessions.Commands;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Results;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Queries;
using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Alexapps.SkinCare.Controllers
{
    [RemoteService]
    public class HomeServiceSessionController : SkinCareController, IHomeServiceSessionService
    {
        private readonly IHomeServiceSessionService _homeServiceSessionService;

        public HomeServiceSessionController(IHomeServiceSessionService homeServiceSessionService)
        {
            _homeServiceSessionService = homeServiceSessionService;
        }

        [HttpPost(ApiRoutes.HomeServiceSessions.Base)]
        public Task<HomeServiceSessionDto> CreateAsync([FromBody] CreateHomeServiceSessionDto input)
        {
            return _homeServiceSessionService.CreateAsync(input);
        }

        [HttpGet(ApiRoutes.HomeServiceSessions.Base)]
        public Task<PagedResultWithMetadata<HomeServiceSessionDto>> GetListAsync([FromQuery] GetHomeServiceSessionsInput input)
        {
            return _homeServiceSessionService.GetListAsync(input);
        }

        [HttpGet(ApiRoutes.HomeServiceSessions.Single)]
        public Task<HomeServiceSessionDto> GetAsync(Guid id)
        {
            return _homeServiceSessionService.GetAsync(id);
        }

        [HttpGet(ApiRoutes.HomeServiceSessions.Schedules)]
        public Task<System.Collections.Generic.List<Alexapps.SkinCare.Dtos.HomeServices.Results.HomeServiceScheduleDto>> GetSchedulesAsync(Guid homeServiceId)
        {
            return _homeServiceSessionService.GetSchedulesAsync(homeServiceId);
        }
    }
}
