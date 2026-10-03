using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Results;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IHomeServiceProviderService : IApplicationService
    {
        Task<PagedResultWithMetadata<HomeServiceProviderDto>> GetListAsync(GetHomeServiceProvidersInput input);
        Task<HomeServiceProviderDto> GetAsync(Guid id);
    }
}
