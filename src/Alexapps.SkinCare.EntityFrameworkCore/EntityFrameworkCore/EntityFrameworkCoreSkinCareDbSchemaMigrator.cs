using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Alexapps.SkinCare.Data;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.EntityFrameworkCore;

public class EntityFrameworkCoreSkinCareDbSchemaMigrator
    : ISkinCareDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreSkinCareDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the SkinCareDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<SkinCareDbContext>()
            .Database
            .MigrateAsync();
    }
}



