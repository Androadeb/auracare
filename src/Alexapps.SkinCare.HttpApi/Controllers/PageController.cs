using Alexapps.SkinCare.Dtos.Pages.Commands;
using Alexapps.SkinCare.Dtos.Pages.Results;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers;

public class PageController(IPageService pageService) : SkinCareController
{
    [HttpGet(ApiRoutes.Pages.Get)]
    [AllowAnonymous]
    public async Task<PageDto> GetAsync([FromQuery] PageTypeEnum type)
    {
        return await pageService.GetAsync(type);
    }
}
