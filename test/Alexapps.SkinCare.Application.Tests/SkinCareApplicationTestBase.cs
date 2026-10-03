using Volo.Abp.Modularity;

namespace Alexapps.SkinCare;

public abstract class SkinCareApplicationTestBase<TStartupModule> : SkinCareTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}



