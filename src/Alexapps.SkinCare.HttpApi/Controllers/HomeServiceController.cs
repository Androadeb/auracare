using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Results;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries;
using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Controllers
{
    public class HomeServiceController(IHomeServiceService homeServiceService) : SkinCareController
    {
        [HttpGet(ApiRoutes.HomeServices.Base)]
        [AllowAnonymous]
        public async Task<PagedResultDto<HomeServiceDto>> GetListAsync([FromQuery] GetHomeServicesInput input)
        {
            return await homeServiceService.GetListAsync(input);
        }

        [HttpGet(ApiRoutes.HomeServices.Single)]
        [AllowAnonymous]
        public async Task<HomeServiceDto> GetAsync(System.Guid id)
        {
            return await homeServiceService.GetAsync(id);
        }

        [HttpGet(ApiRoutes.HomeServices.ServiceProviders)]
        [AllowAnonymous]
        public async Task<PagedResultWithMetadata<HomeServiceProviderDto>> GetServiceProvidersAsync(Guid homeServiceId, [FromQuery] GetHomeServiceProvidersInput input)
        {
            input.HomeServiceId = homeServiceId;
            return await homeServiceService.GetServiceProvidersAsync(input);
        }
    }
}
