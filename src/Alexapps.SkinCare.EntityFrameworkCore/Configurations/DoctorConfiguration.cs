using Alexapps.SkinCare.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");
            builder.ConfigureByConvention();

           
            builder.HasOne(x => x.User).WithOne().HasForeignKey<Doctor>(x => x.UserId);
            builder.HasOne(x => x.Specialty).WithMany().HasForeignKey(x => x.SpecialtyId);

           
            builder.HasMany(x => x.Qualifications)
                   .WithOne()
                   .HasForeignKey(x => x.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasMany(x => x.Blogs)
                   .WithOne(x => x.Author)
                   .HasForeignKey(x => x.AuthorId)
                   .OnDelete(DeleteBehavior.SetNull);

      
            builder.Property(x => x.CanPublishBlogs).HasDefaultValue(false);
            builder.Property(x => x.Rating).IsRequired();

        
        }
    }
}