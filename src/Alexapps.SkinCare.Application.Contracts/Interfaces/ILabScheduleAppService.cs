using Alexapps.SkinCare.Dtos.LabSchedule.Command;
using Alexapps.SkinCare.Dtos.LabSchedule.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface ILabScheduleAppService : IApplicationService
    {
     
        Task<List<LabScheduleResult>> GetListAsync();


        Task<List<LabScheduleResult>> UpdateAllAsync(UpdateAllLabSchedulesDto input);



    }
}