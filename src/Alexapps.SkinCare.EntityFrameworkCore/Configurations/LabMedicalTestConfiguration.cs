using Alexapps.SkinCare.Entities.LABs;
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
    public class LabMedicalTestConfiguration : IEntityTypeConfiguration<LabMedicalTest>
    {
        public void Configure(EntityTypeBuilder<LabMedicalTest> builder)
        {


            builder.ConfigureByConvention();

            // تعريف العلاقات (Navigation Properties)
            builder.HasOne(x => x.Lab)
                   .WithMany()
                   .HasForeignKey(x => x.LabId)
                   .IsRequired();

            builder.HasOne(x => x.MedicalTest)
                   .WithMany()
                   .HasForeignKey(x => x.MedicalTestId)
                   .IsRequired();
        }
    }
}
