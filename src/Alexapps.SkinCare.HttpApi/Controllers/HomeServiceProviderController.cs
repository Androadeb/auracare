using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Results;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers
{
    public class HomeServiceProviderController(IHomeServiceProviderService homeServiceProviderService) : SkinCareController
    {
        [HttpGet(ApiRoutes.HomeServiceProviders.Base)]
        [AllowAnonymous]
        public async Task<PagedResultWithMetadata<HomeServiceProviderDto>> GetListAsync([FromQuery] GetHomeServiceProvidersInput input)
        {
            return await homeServiceProviderService.GetListAsync(input);
        }

        [HttpGet(ApiRoutes.HomeServiceProviders.Single)]
        [AllowAnonymous]
        public async Task<HomeServiceProviderDto> GetAsync(Guid id)
        {
            return await homeServiceProviderService.GetAsync(id);
        }
    }
}
