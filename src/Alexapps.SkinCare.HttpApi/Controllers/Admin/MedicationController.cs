using Alexapps.SkinCare.Dtos.Medications.Commands;
using Alexapps.SkinCare.Dtos.Medications.Results;
using Alexapps.SkinCare.Interfaces.Medications;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers.Admin
{
    [Authorize]
    public class MedicationController(IMedicationService medicationService) : SkinCareController
    {
        [HttpPost(ApiRoutes.Admin.Medications.Base)]
        public async Task<MedicationDto> CreateAsync([FromBody] CreateUpdateMedicationDto input)
        {
            return await medicationService.CreateAsync(input);
        }

        [HttpPut(ApiRoutes.Admin.Medications.Single)]
        public async Task<MedicationDto> UpdateAsync(Guid id, [FromBody] CreateUpdateMedicationDto input)
        {
            return await medicationService.UpdateAsync(id, input);
        }

        [HttpDelete(ApiRoutes.Admin.Medications.Single)]
        public async Task DeleteAsync(Guid id)
        {
            await medicationService.DeleteAsync(id);
        }
    }
}
