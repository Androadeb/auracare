using Volo.Abp.Modularity;

namespace Alexapps.SkinCare;

[DependsOn(
    typeof(SkinCareApplicationModule),
    typeof(SkinCareDomainTestModule)
)]
public class SkinCareApplicationTestModule : AbpModule
{

}



