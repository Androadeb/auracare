using Alexapps.SkinCare.Entities.HomeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.Configurations
{
    public class HomeServiceProviderConfiguration : IEntityTypeConfiguration<HomeServiceProvider>
    {
        public void Configure(EntityTypeBuilder<HomeServiceProvider> builder)
        {
            builder.ConfigureByConvention();
            builder.HasOne(x => x.Doctor).WithMany().HasForeignKey(x => x.DoctorId);
            builder.HasOne(x => x.HomeService).WithMany().HasForeignKey(x => x.HomeServiceId);
        }
    }
}
