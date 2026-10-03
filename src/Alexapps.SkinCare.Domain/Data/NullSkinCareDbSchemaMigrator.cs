using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.Data;

/* This is used if database provider does't define
 * ISkinCareDbSchemaMigrator implementation.
 */
public class NullSkinCareDbSchemaMigrator : ISkinCareDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}



