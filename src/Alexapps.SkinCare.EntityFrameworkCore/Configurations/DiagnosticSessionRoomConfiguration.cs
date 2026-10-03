using Alexapps.SkinCare.Entities.Consultations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Configurations
{
    public class DiagnosticSessionRoomConfiguration : IEntityTypeConfiguration<DiagnosticSessionRoom>
    {
        public void Configure(EntityTypeBuilder<DiagnosticSessionRoom> builder)
        {

            builder.ToTable("DiagnosticSessionRooms");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.VideoRoomId)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.DoctorJoinUrl)
                .IsRequired();

            builder.Property(x => x.PatientJoinUrl)
                .IsRequired();

          
          
            builder.HasOne(x => x.DiagnosticSession)
                .WithMany(x => x.VideoRooms) 
                .HasForeignKey(x => x.DiagnosticSessionId)
                .OnDelete(DeleteBehavior.Cascade); 
           
            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
