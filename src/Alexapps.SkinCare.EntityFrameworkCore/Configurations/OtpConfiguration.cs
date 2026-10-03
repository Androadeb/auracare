using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Alexapps.SkinCare.EntityConfigurations;

internal class OtpConfiguration : IEntityTypeConfiguration<Otp>
{
    public void Configure(EntityTypeBuilder<Otp> builder)
    {
        builder.HasOne(x => x.User)
            .WithOne(x => x.Otp)
            .HasForeignKey<Otp>(x => x.UserId).IsRequired();
            

    }
}

