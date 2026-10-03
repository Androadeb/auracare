using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Results;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries;
using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IHomeServiceService : IApplicationService
    {
        Task<PagedResultDto<HomeServiceDto>> GetListAsync(GetHomeServicesInput input);
        Task<HomeServiceDto> GetAsync(System.Guid id);
        Task<PagedResultWithMetadata<HomeServiceProviderDto>> GetServiceProvidersAsync(GetHomeServiceProvidersInput input);
    }
}
