using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Commands;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Results;
using Alexapps.SkinCare.Dtos.HomeServiceSessions.Queries;
using Alexapps.SkinCare.Dtos.Common;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IHomeServiceSessionService : IApplicationService
    {
        Task<HomeServiceSessionDto> CreateAsync(CreateHomeServiceSessionDto input);
        Task<HomeServiceSessionDto> GetAsync(Guid id);
        Task<List<HomeServiceScheduleDto>> GetSchedulesAsync(Guid homeServiceId);
        Task<PagedResultWithMetadata<HomeServiceSessionDto>> GetListAsync(GetHomeServiceSessionsInput input);
    }
}
