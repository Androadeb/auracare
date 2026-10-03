using Alexapps.SkinCare.Entities.LABs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.Configurations
{
    public class LabScheduleConfiguration : IEntityTypeConfiguration<LabSchedule>
    {
        public void Configure(EntityTypeBuilder<LabSchedule> builder)
        {
            builder.ToTable("LabSchedules");
            builder.ConfigureByConvention();

            // Mapping the properties
            builder.Property(x => x.LabId).IsRequired();

            // Correctly define the relationship to use LabId for the Lab navigation property
            builder.HasOne(x => x.Lab) 
                   .WithMany()    
                   .HasForeignKey(x => x.LabId) 
                   .IsRequired();
            builder.Property(x => x.IsOpen).HasDefaultValue(true);
        }
    }
}