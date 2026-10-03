using Alexapps.SkinCare.Entities.Users;
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
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            // Set table name and schema
            builder.ToTable("AppUserDevices");

            // ABP standard configuration (handles base properties)
            builder.ConfigureByConvention();

            // Property configurations
            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(512); // Sufficient length for FCM tokens

            builder.Property(x => x.Language)
                .HasMaxLength(10)
                .HasDefaultValue("en"); // Default language for the device

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            // Indexes for performance
            builder.HasIndex(x => x.UserId); // Important for finding user devices
            builder.HasIndex(x => x.Token);  // Important for checking existing tokens
        }
    }
}
