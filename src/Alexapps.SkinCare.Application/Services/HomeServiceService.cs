using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Results;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries;
using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using Alexapps.SkinCare.Entities.HomeServices;
using Alexapps.SkinCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services
{
    public class HomeServiceService(
        IRepository<HomeService, Guid> homeServiceRepository,
        IHomeServiceProviderService homeServiceProviderService) : SkinCareAppService, IHomeServiceService
    {
        private readonly IRepository<HomeService, Guid> _homeServiceRepository = homeServiceRepository;

        public async Task<PagedResultDto<HomeServiceDto>> GetListAsync(GetHomeServicesInput input)
        {
            var queryable = await _homeServiceRepository.GetQueryableAsync();

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var homeServices = await AsyncExecuter.ToListAsync(
                queryable.Skip(input.SkipCount).Take(input.MaxResultCount)
            );

            var dtos = ObjectMapper.Map<List<HomeService>, List<HomeServiceDto>>(homeServices);
            
            return new PagedResultDto<HomeServiceDto>(totalCount, dtos);
        }

        public async Task<HomeServiceDto> GetAsync(Guid id)
        {
            var homeService = await _homeServiceRepository.GetAsync(id);
            return ObjectMapper.Map<HomeService, HomeServiceDto>(homeService);
        }

        public async Task<PagedResultWithMetadata<HomeServiceProviderDto>> GetServiceProvidersAsync(GetHomeServiceProvidersInput input)
        {
            return await homeServiceProviderService.GetListAsync(input);
        }
    }
}
