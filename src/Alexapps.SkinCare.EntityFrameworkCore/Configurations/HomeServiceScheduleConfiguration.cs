using Alexapps.SkinCare.Entities.HomeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.EntityFrameworkCore.Configurations
{
    public class HomeServiceScheduleConfiguration : IEntityTypeConfiguration<HomeServiceSchedule>
    {
        public void Configure(EntityTypeBuilder<HomeServiceSchedule> builder)
        {
            builder.ToTable(SkinCareConsts.DbTablePrefix + "HomeServiceSchedules", SkinCareConsts.DbSchema);
            builder.ConfigureByConvention();

            builder.HasOne(x => x.HomeService)
                   .WithMany()
                   .HasForeignKey(x => x.HomeServiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
