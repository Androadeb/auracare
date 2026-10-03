using Alexapps.SkinCare.Dtos.Pages.Commands;
using Alexapps.SkinCare.Dtos.Pages.Results;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // For Tags and others if needed
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers.Admin
{
    [Authorize(Roles = RoleEnum.ADMIN)]
    [Tags("Admin Pages")]
    public class PagesController(IPageService pageService) : SkinCareController
    {
        [HttpGet(ApiRoutes.Admin.Pages.Base)]
        public async Task<ActionResult<PageResult>> GetAsync([FromQuery] PageTypeEnum type)
        {
            var response = await pageService.GetForEditAsync(type);
            return Ok(response);
        }

        [HttpPut(ApiRoutes.Admin.Pages.Base)]
        public async Task<ActionResult<PageResult>> UpdateAsync([FromQuery] PageTypeEnum type, [FromBody] PageCommand command)
        {
            var response = await pageService.UpdateAsync(type, command);
            return Ok(response);
        }
    }
}

