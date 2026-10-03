using Alexapps.SkinCare.Dtos.SkinConditions.Results;
using Alexapps.SkinCare.Entities.SkinConditions;
using Alexapps.SkinCare.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services
{
    public class SkinConditionService : SkinCareAppService, ISkinConditionService
    {
        private readonly IRepository<SkinCondition, Guid> _skinConditionRepository;

        public SkinConditionService(IRepository<SkinCondition, Guid> skinConditionRepository)
        {
            _skinConditionRepository = skinConditionRepository;
        }

        public async Task<List<SkinConditionDto>> GetListAsync()
        {
            var skinConditions = await _skinConditionRepository.GetListAsync();
            return ObjectMapper.Map<List<SkinCondition>, List<SkinConditionDto>>(skinConditions);
        }
    }
}
