using Microsoft.Extensions.Localization;
using Alexapps.SkinCare.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Alexapps.SkinCare;

[Dependency(ReplaceServices = true)]
public class SkinCareBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<SkinCareResource> _localizer;

    public SkinCareBrandingProvider(IStringLocalizer<SkinCareResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}



