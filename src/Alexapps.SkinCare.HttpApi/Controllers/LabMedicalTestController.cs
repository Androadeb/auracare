using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Command;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Queries;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Result;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Controllers
{
    [RemoteService(Name = "Lab")]
    [Area("lab")]
    public class LabMedicalTestController : SkinCareController
    {
        private readonly ILabMedicalTestAppService _labMedicalTestAppService;

        public LabMedicalTestController(ILabMedicalTestAppService labMedicalTestAppService)
        {
            _labMedicalTestAppService = labMedicalTestAppService;
        }
        [HttpGet(ApiRoutes.LabMedicalTests.Base)]
        public virtual Task<PagedResultWithMetadata<LabMedicalTestDto>> GetListAsync([FromQuery] GetLabMedicalTestListDto input)
        {
            return _labMedicalTestAppService.GetListAsync(input);
        }

        [HttpPost(ApiRoutes.LabMedicalTests.Base)]
        public virtual Task<LabMedicalTestDto> CreateAsync([FromBody] CreateLabMedicalTestDto command)
        {
            return _labMedicalTestAppService.CreateAsync(command);
        }

       
        [HttpPut(ApiRoutes.LabMedicalTests.Single)]
        public virtual Task<LabMedicalTestDto> UpdateAsync(Guid id, [FromBody] UpdateLabMedicalTestDto command)
        {
           
            return _labMedicalTestAppService.UpdateAsync(id, command);
        }

       
        [HttpDelete(ApiRoutes.LabMedicalTests.Single)]
        public virtual Task DeleteAsync(Guid id)
        {
           
            return _labMedicalTestAppService.DeleteAsync(new DeleteLabMedicalTestDto { Id = id });
        }
    }
}