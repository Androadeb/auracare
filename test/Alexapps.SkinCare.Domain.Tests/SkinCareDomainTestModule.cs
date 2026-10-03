using Volo.Abp.Modularity;

namespace Alexapps.SkinCare;

[DependsOn(
    typeof(SkinCareDomainModule),
    typeof(SkinCareTestBaseModule)
)]
public class SkinCareDomainTestModule : AbpModule
{

}



