using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Identity;

namespace Alexapps.SkinCare.Entities.Users;

public class User : IdentityUser
{
    //private readonly HashSet<RefreshToken> _refreshTokens = new HashSet<RefreshToken>();  // Correct initialization

    private User()
    {
    }

    public User(string name, string phone, string? email, string userName)
    {
        Id = Guid.NewGuid();
        Name = Check.NotNull(name, nameof(name));
        PhoneNumber = phone;
        UserName = userName;
        NormalizedUserName = userName?.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
        SecurityStamp = Guid.NewGuid().ToString();
        Email = email;
        NormalizedEmail = email?.ToUpperInvariant();
        IsActive = false;
    }

    public Otp Otp { get; set; }
    public  ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public string? ProfileImage { get; set; }
    public string? Gender { get; set; } // "male" or "female"

    public void Activate()
    {
        IsActive = true;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        UserName = phoneNumber;
        NormalizedUserName = phoneNumber.ToUpperInvariant();
        IsActive = false;
    }
    public void SetEmail(string email)
    {
        Email = email;
        NormalizedEmail = email?.ToUpperInvariant();
        EmailConfirmed = true; // Mark as confirmed since it's verified by OTP
    }
    public void UpdateProfile(string name, string? email, string? gender)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        Email = email;
        NormalizedEmail = email?.ToUpperInvariant();
        Gender = gender;
    }

    public void SetLockoutEnabled(bool enabled)
    {
        LockoutEnabled = enabled;
    }
}

