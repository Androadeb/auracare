using Alexapps.SkinCare.Data;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.Seeders
{
    public class DatabaseSeeder(
    PagesDataSeeder pagesDataSeeder,
    UserRolesDataSeeder userRolesSeeder,
    UsersDataSeeder usersDataSeeder,
    DoctorsAndHomeServicesDataSeeder doctorsAndHomeServicesDataSeeder,
    SkinConditionsDataSeeder skinConditionsDataSeeder,
    MedicalTestsDataSeeder medicalTestsDataSeeder,
    MedicationsDataSeeder medicationsDataSeeder,
    LABsSeeder labsSeeder, LabSchedulesDataSeeder labSchedulesDataSeeder, DoctorSchedulesDataSeeder doctorSchedulesDataSeeder
) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {
            await userRolesSeeder.SeedAsync(context);
            await labsSeeder.SeedAsync(context);
            await labSchedulesDataSeeder.SeedAsync(context);
            await pagesDataSeeder.SeedAsync(context);
          
            await doctorsAndHomeServicesDataSeeder.SeedAsync(context);
            await skinConditionsDataSeeder.SeedAsync(context);
            await medicalTestsDataSeeder.SeedAsync(context);
            await medicationsDataSeeder.SeedAsync(context);
            await doctorSchedulesDataSeeder.SeedAsync(context);
        }
    }
}
