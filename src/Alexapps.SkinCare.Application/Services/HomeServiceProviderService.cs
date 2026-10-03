using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Results;
using Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries;
using Alexapps.SkinCare.Entities.HomeServices;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services
{
    public class HomeServiceProviderService : SkinCareAppService, IHomeServiceProviderService
    {
        private readonly IRepository<HomeServiceProvider, Guid> _homeServiceProviderRepository;

        public HomeServiceProviderService(IRepository<HomeServiceProvider, Guid> homeServiceProviderRepository)
        {
            _homeServiceProviderRepository = homeServiceProviderRepository;
        }

        public async Task<PagedResultWithMetadata<HomeServiceProviderDto>> GetListAsync(GetHomeServiceProvidersInput input)
        {
            var queryable = await _homeServiceProviderRepository.WithDetailsAsync(x => x.Doctor, x => x.Doctor.User, x => x.Doctor.Specialty);
            
            // Filter by ServiceProvider type just in case
            queryable = queryable.Where(x => x.Doctor.Type == DoctorTypeEnum.ServiceProvider);

            if (!string.IsNullOrWhiteSpace(input.Name))
            {
                queryable = queryable.Where(x => x.Doctor.User.Name.Contains(input.Name));
            }

            if (input.Gender.HasValue)
            {
                var genderString = input.Gender.Value == UserGenderEnum.Male ? "male" : "female";
                queryable = queryable.Where(x => x.Doctor.User.Gender == genderString);
            }

            if (input.HomeServiceId.HasValue)
            {
                queryable = queryable.Where(x => x.HomeServiceId == input.HomeServiceId.Value);
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            var skipCount = (input.Page - 1) * input.Limit;

            var providers = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(x => x.Doctor.User.Name).Skip(skipCount).Take(input.Limit)
            );

            var dtos = ObjectMapper.Map<List<HomeServiceProvider>, List<HomeServiceProviderDto>>(providers);

            return new PagedResultWithMetadata<HomeServiceProviderDto>(dtos, input.Page, input.Limit, totalCount);
        }

        public async Task<HomeServiceProviderDto> GetAsync(Guid id)
        {
            var queryable = await _homeServiceProviderRepository.WithDetailsAsync(x => x.Doctor, x => x.Doctor.User, x => x.Doctor.Specialty);
            var provider = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));
            
            return ObjectMapper.Map<HomeServiceProvider, HomeServiceProviderDto>(provider);
        }
    }
}
