using Alexapps.SkinCare.Dtos.Medications.Results;
using Alexapps.SkinCare.Interfaces.Medications;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Controllers
{
    [Authorize]
    public class MedicationController(IMedicationService medicationService) : SkinCareController
    {
        [HttpGet(ApiRoutes.Medications.Base)]
        public async Task<PagedResultDto<MedicationDto>> GetListAsync([FromQuery] string keyword, PagedAndSortedResultRequestDto input)
        {
            return await medicationService.GetListAsync(keyword, input);
        }

        [HttpGet(ApiRoutes.Medications.Single)]
        public async Task<MedicationDto> GetAsync(Guid id)
        {
            return await medicationService.GetAsync(id);
        }
    }
}
