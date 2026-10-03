using Alexapps.SkinCare.Entities.SkinConditions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.Configurations
{
    public class SkinConditionConfiguration : IEntityTypeConfiguration<SkinCondition>
    {
        public void Configure(EntityTypeBuilder<SkinCondition> builder)
        {
            builder.ConfigureByConvention();

            builder.Property(x => x.NameAr).IsRequired();
            builder.Property(x => x.NameEn).IsRequired();
            builder.Property(x => x.Image).IsRequired(false);
            builder.Property(x => x.DescriptionAr).IsRequired(false);
            builder.Property(x => x.DescriptionEn).IsRequired(false);
        }
    }
}
