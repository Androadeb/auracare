using Alexapps.SkinCare.Entities.Notifications;
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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // Set table name
            builder.ToTable("AppNotifications");

            // ABP standard configuration
            builder.ConfigureByConvention();

            // Property configurations
            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Body)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.TargetType)
                .HasMaxLength(64); // Example: "Chat", "LabOrder"

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            // Indexes for performance
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreationTime); // Optimized for "Order by Date"
        }
    }
}
