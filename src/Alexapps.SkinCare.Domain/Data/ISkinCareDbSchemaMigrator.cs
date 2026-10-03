using System.Threading.Tasks;

namespace Alexapps.SkinCare.Data;

public interface ISkinCareDbSchemaMigrator
{
    Task MigrateAsync();
}



