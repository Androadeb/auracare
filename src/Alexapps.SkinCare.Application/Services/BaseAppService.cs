using Alexapps.SkinCare.Localization;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare;

/* Inherit your application services from this class.
 */
public abstract class BaseAppService : ApplicationService
{
    protected BaseAppService()
    {
        LocalizationResource = typeof(SkinCareResource);
    }
}



