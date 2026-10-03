using Alexapps.SkinCare.Entities.Consultations;
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
    public class DoctorRatingConfiguration : IEntityTypeConfiguration<DoctorRating>
    {
        public void Configure(EntityTypeBuilder<DoctorRating> builder)
        {
            // Set Table Name
            builder.ToTable("DoctorRatings");

            // Configure ABP Basic Properties (Id, CreationTime, etc.)
            builder.ConfigureByConvention();

            // Property Constraints
            builder.Property(x => x.Stars)
                   .IsRequired();

            builder.Property(x => x.Comment)
                   .HasMaxLength(1000) // English comment support as per project rules
                   .IsRequired(false);

            // Relationships

            // 1. Relationship with Doctor
            builder.HasOne(x => x.Doctor)
                   .WithMany() // You can add ICollection<DoctorRating> to Doctor class later if needed
                   .HasForeignKey(x => x.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            // 2. Relationship with DiagnosticSession
            builder.HasOne<DiagnosticSession>()
                   .WithMany()
                   .HasForeignKey(x => x.DiagnosticSessionId)
                   .OnDelete(DeleteBehavior.NoAction) // Prevent multiple cascade paths
                   .IsRequired();

            // 3. Relationship with User (Patient)
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired();
        }
    }
}
