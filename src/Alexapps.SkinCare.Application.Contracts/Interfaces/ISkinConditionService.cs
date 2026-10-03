using Alexapps.SkinCare.Dtos.SkinConditions.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface ISkinConditionService : IApplicationService
    {
        Task<List<SkinConditionDto>> GetListAsync();
    }
}
