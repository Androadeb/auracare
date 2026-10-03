using Alexapps.SkinCare.Dtos.LabSchedule.Command;
using Alexapps.SkinCare.Dtos.LabSchedule.Result;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers.LabSchedules
{
    [ApiExplorerSettings(GroupName = "Lab")]
    public class LabScheduleController : AbpController
    {
        private readonly ILabScheduleAppService _labScheduleAppService;

        public LabScheduleController(ILabScheduleAppService labScheduleAppService)
        {
            _labScheduleAppService = labScheduleAppService;
        }

     
        [HttpGet(ApiRoutes.LabSchedules.Base + "my-schedule")]
        public virtual Task<List<LabScheduleResult>> GetListAsync()
        {
            return _labScheduleAppService.GetListAsync();
        }

       
        [HttpPut(ApiRoutes.LabSchedules.Base + "bulk-update")]
        public virtual async Task<List<LabScheduleResult>> UpdateAllAsync([FromBody] UpdateAllLabSchedulesDto input)
        {
            return await _labScheduleAppService.UpdateAllAsync(input);
        }

     
    }
}