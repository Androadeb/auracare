using Alexapps.SkinCare.Controllers;
using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.LabBranches.Commands;
using Alexapps.SkinCare.Dtos.LabBranches.Queries;
using Alexapps.SkinCare.Dtos.LabBranches.Result;
using Alexapps.SkinCare.LabBranches;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

[RemoteService(Name = "LabBranch")]
[Area("app")]
// التعديل هنا: استخدام الـ ApiRoutes
[Route(ApiRoutes.LabBranches.Base)]
public class LabBranchController : SkinCareController, ILabBranchAppService
{
    private readonly ILabBranchAppService _labBranchAppService;

    public LabBranchController(ILabBranchAppService labBranchAppService)
    {
        _labBranchAppService = labBranchAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<LabBranchDto> GetAsync(Guid id)
    {
        return _labBranchAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("my-branches")]
    public virtual Task<PagedResultWithMetadata<LabBranchDto>> GetMyBranchesAsync(GetLabBranchesInput input)
    {
        return _labBranchAppService.GetMyBranchesAsync(input);
    }

    [HttpPost] // api/v1/lab/branches/
    public virtual Task<LabBranchDto> CreateAsync(CreateLabBranchDto input)
    {
        return _labBranchAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")] 
    public virtual Task<LabBranchDto> UpdateAsync(Guid id, UpdateLabBranchDto input)
    {
        return _labBranchAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")] 
    public virtual Task DeleteAsync(Guid id)
    {
        return _labBranchAppService.DeleteAsync(id);
    }
}