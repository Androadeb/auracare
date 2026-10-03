using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Alexapps.SkinCare.Entities.HomeServices;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.EntityFrameworkCore.Configurations
{
    public class HomeServiceSessionConfiguration : IEntityTypeConfiguration<HomeServiceSession>
    {
        public void Configure(EntityTypeBuilder<HomeServiceSession> builder)
        {
            builder.ToTable(SkinCareConsts.DbTablePrefix + "HomeServiceSessions", SkinCareConsts.DbSchema);
            builder.ConfigureByConvention();

            builder.HasOne(x => x.HomeService)
                   .WithMany()
                   .HasForeignKey(x => x.HomeServiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Doctor)
                   .WithMany()
                   .HasForeignKey(x => x.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Assuming we don't have a direct navigation property to User or Customer yet, 
            // but if we did:
            // builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        }
    }
}
