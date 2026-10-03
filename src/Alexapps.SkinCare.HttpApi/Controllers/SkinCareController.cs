using Alexapps.SkinCare.Localization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers
{
    [IgnoreAntiforgeryToken]
    public abstract class SkinCareController : AbpControllerBase
    {
        protected SkinCareController()
        {
            LocalizationResource = typeof(SkinCareResource);
        }
    }

}


