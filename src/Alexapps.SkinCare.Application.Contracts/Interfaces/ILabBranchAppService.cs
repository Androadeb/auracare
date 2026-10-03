using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.LabBranches.Commands;
using Alexapps.SkinCare.Dtos.LabBranches.Queries;
using Alexapps.SkinCare.Dtos.LabBranches.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.LabBranches;

public interface ILabBranchAppService : IApplicationService
{
  
    Task<LabBranchDto> GetAsync(Guid id);


    Task<PagedResultWithMetadata<LabBranchDto>> GetMyBranchesAsync(GetLabBranchesInput input);

    Task<LabBranchDto> CreateAsync(CreateLabBranchDto input);

  
    Task<LabBranchDto> UpdateAsync(Guid id, UpdateLabBranchDto input);

   
    Task DeleteAsync(Guid id);
}