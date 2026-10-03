using Alexapps.SkinCare.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Alexapps.SkinCare.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SkinCareEntityFrameworkCoreModule),
    typeof(SkinCareApplicationContractsModule)
    )]
public class SkinCareDbMigratorModule : AbpModule
{
}



