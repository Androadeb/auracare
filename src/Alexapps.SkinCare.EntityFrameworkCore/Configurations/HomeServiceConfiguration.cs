using Alexapps.SkinCare.Entities.HomeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.Configurations
{
    public class HomeServiceConfiguration : IEntityTypeConfiguration<HomeService>
    {
        public void Configure(EntityTypeBuilder<HomeService> builder)
        {
            builder.ConfigureByConvention();

            builder.Property(x => x.NameAr).IsRequired();
            builder.Property(x => x.NameEn).IsRequired();
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Duration).IsRequired();
            builder.Property(x => x.DescriptionAr).IsRequired();
            builder.Property(x => x.DescriptionEn).IsRequired();
            builder.Property(x => x.ServicesAr).IsRequired();
            builder.Property(x => x.ServicesEn).IsRequired();
            builder.Property(x => x.NotesAr).IsRequired();
            builder.Property(x => x.NotesEn).IsRequired();
            builder.Property(x => x.Image).IsRequired(false);
        }
    }
}
