using Alexapps.SkinCare.Entities.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Alexapps.SkinCare.Configurations
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable(SkinCareConsts.DbTablePrefix + "Blogs", SkinCareConsts.DbSchema);
            builder.ConfigureByConvention();
            builder.Property(x => x.Title).IsRequired().HasMaxLength(256);
            builder.Property(x => x.ImageUrl).HasMaxLength(512);

          
            builder.HasOne(x => x.Author)
                   .WithMany()
                   .HasForeignKey(x => x.AuthorId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
