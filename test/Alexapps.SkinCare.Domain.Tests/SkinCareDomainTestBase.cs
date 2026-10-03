using Volo.Abp.Modularity;

namespace Alexapps.SkinCare;

/* Inherit from this class for your domain layer tests. */
public abstract class SkinCareDomainTestBase<TStartupModule> : SkinCareTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}



