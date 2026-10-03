using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.HomeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.EntityFrameworkCore.Configurations
{
    public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.ToTable(SkinCareConsts.DbTablePrefix + "DoctorSchedules", SkinCareConsts.DbSchema);
            builder.ConfigureByConvention();

            builder.HasOne(x => x.Doctor)
                   .WithMany()
                   .HasForeignKey(x => x.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
