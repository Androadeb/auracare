using Alexapps.SkinCare.Dtos.SkinConditions.Results;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers
{
    public class SkinConditionController(ISkinConditionService skinConditionService) : SkinCareController
    {
        [HttpGet(ApiRoutes.SkinConditions.Base)]
        [AllowAnonymous]
        public async Task<List<SkinConditionDto>> GetListAsync()
        {
            return await skinConditionService.GetListAsync();
        }
    }
}
