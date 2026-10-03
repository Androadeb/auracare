using Alexapps.SkinCare.Business.Client.Profile.Results;
using Alexapps.SkinCare.Business.OTP.Commands.SendOtp;
using Alexapps.SkinCare.Business.OTP.Commands.VerifyOtp;
using Alexapps.SkinCare.Business.OTP.Results;
using Alexapps.SkinCare.Configurations;
using Alexapps.SkinCare.Dtos.Auth.Commands.Login;
using Alexapps.SkinCare.Dtos.Auth.Commands.RefreshToken;
using Alexapps.SkinCare.Dtos.Auth.Commands.Register;
using Alexapps.SkinCare.Dtos.Auth.Commands.UpdateProfile;
using Alexapps.SkinCare.Dtos.Auth.Results.Login;
using Alexapps.SkinCare.Dtos.Auth.Results.Register;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.LabBranches.Result;
using Alexapps.SkinCare.Dtos.OTP.Commands.VerifyOtp;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Helpers;
using Alexapps.SkinCare.Integrations.Storage;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using User = Alexapps.SkinCare.Entities.Users.User;

namespace Alexapps.SkinCare.Services;

public class AuthService(
     IRepository<RefreshToken, Guid> refreshTokenRepository,
    IRepository<User, Guid> userRepository,
    IRepository<Otp, Guid> otpRepository,
    IdentityUserManager userManager,
    JwtConfiguration jwtConfig,
    IStringLocalizer<SkinCareResource> localizer,
    IdentityUserManager identityUserManager, IRepository<UserDevice, Guid> userDeviceRepository, IRepository<Alexapps.SkinCare.Entities.Notifications.Notification, Guid> notificationRepository,
    IStorageService storageService,
    ICurrentUser currentUser, IRepository<Doctor, Guid> doctorRepository, 
    IRepository<Lab, Guid> labRepository,
    IUnitOfWorkManager _unitOfWorkManager
    ) : BaseAppService, IAuthService
{
    [UnitOfWork]
    public async Task<RegisterResult> RegisterAsync(RegisterCommand Command)
    {

        // Declare the variable to hold the newly created user
        User userCreated;


        // Check if the phone number already exists
        User userFind = await userRepository.FirstOrDefaultAsync(x => x.PhoneNumber == Command.PhoneNumber);
        if (userFind != null)
        {
            // If the phone number already exists, throw an exception
            throw new UserFriendlyException(localizer["phone_number_already_exist"], "400");
        }

        // Check if the email already exists
        if (!string.IsNullOrEmpty(Command.Email))
        {
            User emailCheck = await userRepository.FirstOrDefaultAsync(x => x.Email == Command.Email);
            if (emailCheck != null)
            {
                // If the email already exists, throw an exception
                throw new UserFriendlyException(localizer["email_already_exist"], "400");
            }
        }

        // Create the user object with the provided phone number
        userCreated = new User(Command.Name, Command.PhoneNumber, Command.Email, Command.PhoneNumber);
        userCreated.SetLockoutEnabled(true);

        if (Command.UserGender != UserGenderEnum.Male && Command.UserGender != UserGenderEnum.Female)
        {
           // Handle if needed, or default
        }
        userCreated.Gender = Command.UserGender.ToString().ToLowerInvariant();


        // Create the user in the Identity system
        var createUserResult = await identityUserManager.CreateAsync(userCreated);

        // Check if user creation failed
        if (!createUserResult.Succeeded)
        {
            // If user creation fails, throw an exception with the error message
            throw new UserFriendlyException(localizer["failed_to_create_user"], "400", createUserResult.Errors.FirstOrDefault()?.Description);
        }

        // Assign the "Client" role to the newly created user
        var assignRoleResult = await identityUserManager.AddToRoleAsync(userCreated, RoleEnum.CLIENT.ToString());

        // Check if role assignment failed
        if (!assignRoleResult.Succeeded)
        {
            // If role assignment fails, collect error messages and throw an exception
            var errorMessage = string.Join(", ", assignRoleResult.Errors.Select(e => e.Description));
            throw new UserFriendlyException(localizer["failed_to_assign_role"], "400", errorMessage);
        }

        // Generate OTP for phone/email verification
        var otp = new Otp
        {
            UserId = userCreated.Id,
            PhoneCode = GenerateOtp.GetOtp(),
            PhoneCodeExpireAt = DateTime.UtcNow.AddMinutes(1),
            Type = OtpTypeEnum.ConfirmPhone
        };

        // Insert OTP into the database
        var otpSaved = await otpRepository.InsertAsync(otp, true);

        // Handle profile image if provided
        if (Command.ProfileImage != null && Command.ProfileImage.Length > 0)
        {
            // Upload the profile image to storage
            var url = await storageService.Upload(Command.ProfileImage);
            
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                userCreated.ProfileImage = uri.PathAndQuery;
            }
            else
            {
                userCreated.ProfileImage = url;
            }
        }

        await userRepository.UpdateAsync(userCreated);

        // Return the result with user ID, phone number, email, role, and OTP expiry time
        return new RegisterResult
        {
            Id = userCreated.Id,
            PhoneNumber = userCreated.PhoneNumber,
            Role = RoleEnum.CLIENT.ToString(),
            PhoneCodeExpireAt = (int)(otp.PhoneCodeExpireAt - DateTime.UtcNow).TotalSeconds,  // Return OTP expiration in seconds
        };
    }
    public async Task<SendOtpResult> SendOTPAsync(SendOtpCommand Command)
    {
        User userFind;


        // Find user by phone number or email
        userFind = await userRepository.WithDetailsAsync(x => x.Otp)
                                       .Result
                                       .FirstOrDefaultAsync(x => x.PhoneNumber == Command.Username || x.Email == Command.Username);
        if (userFind == null)
        {
            throw new UserFriendlyException(localizer["user_not_exist"], "400"); // User not found
        }

        if (!await userManager.IsInRoleAsync(userFind, Command.Role))
        {
            throw new UserFriendlyException(localizer["user_not_in_role"], "400");
        }



        // Step 2: Delete existing OTP if it exists
        if (userFind.Otp != null)
        {


            // Hard delete the existing OTP
            await otpRepository.HardDeleteAsync(userFind.Otp, true);
        }

        // Step 3: Generate a new OTP
        var newOtp = new Otp
        {

            UserId = userFind.Id,
            PhoneCode = GenerateOtp.GetOtp(),
            PhoneCodeExpireAt = DateTime.UtcNow.AddMinutes(1),
            Type = OtpTypeEnum.ConfirmEmail


        };

        // Step 4: Save the new OTP
        await otpRepository.InsertAsync(newOtp);

        // Step 5: Return the result with OTP details
        return new SendOtpResult
        {
            ExpiryTimeByMinute = newOtp.PhoneCodeExpireAt, // Expiration time of the OTP
            OtpCode = newOtp.PhoneCode, // Generated OTP code
            Email = userFind.Email
        };
    }


    [UnitOfWork]
    public async Task<AuthResult> VerifyOTPAsync(VerifyOtpCommand Command)
    {

        // Step 1: Find the user based on AuthType (Phone or Email)
        User userFind;


        // Fetch user by phone number or email with OTP and RefreshTokens details
        userFind = await userRepository.WithDetailsAsync(x => x.Otp, x => x.RefreshTokens)
                                       .Result
                                       .FirstOrDefaultAsync(x => x.PhoneNumber == Command.Username || x.Email == Command.Username);



        // Step 2: Handle user not found case
        if (userFind == null)
        {
            throw new UserFriendlyException(localizer["user_not_exist"], "400"); // User does not exist
        }

        // Step 3: Check if OTP exists for the user
        if (userFind.Otp == null)
        {
            throw new UserFriendlyException(localizer["otp_not_sent"], "400"); // OTP not sent
        }

        // Check if user is locked out
        if (await userManager.IsLockedOutAsync(userFind))
        {
            throw new UserFriendlyException(localizer["account_locked_contact_admin"], "403");
        }

        // Step 4: Validate OTP code
        if (userFind.Otp.PhoneCode != Command.Otp)
        {
            // Increment failed access count in a separate transaction
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true))
            {
                var userForFail = await userManager.FindByIdAsync(userFind.Id.ToString());
                if (userForFail != null)
                {
                    await userManager.AccessFailedAsync(userForFail);

                    if (await userManager.GetAccessFailedCountAsync(userForFail) >= 5)
                    {
                        await userManager.SetLockoutEndDateAsync(userForFail, DateTimeOffset.MaxValue);
                        await uow.CompleteAsync(); // Commit before throwing
                        throw new UserFriendlyException(localizer["account_locked_too_many_attempts"], "403");
                    }
                }
                await uow.CompleteAsync(); // Commit changes
            }

            throw new UserFriendlyException(localizer["invalid_otp"], "400"); // Invalid OTP code
        }

        // Reset failed access count on success
        await userManager.ResetAccessFailedCountAsync(userFind);

        // Step 5: Check if OTP has expired
        if (userFind.Otp.PhoneCodeExpireAt < DateTime.UtcNow)
        {
            throw new UserFriendlyException(localizer["otp_expired"], "400"); // OTP expired
        }

        // Step 6: Activate the user if inactive
        if (!userFind.IsActive)
        {
            userFind.Activate(); // Activate user if not already active
        }

        // Step 7: Map the user entity to AuthResult DTO
        var result = ObjectMapper.Map<User, AuthResult>(userFind);

        var userRoles = await userManager.GetRolesAsync(userFind); // Assuming GetRolesAsync fetches roles for the user
        result.Role = userRoles[0];

        // Step 8: Generate and assign access token
        result.AccessToken = await GenerateTokenAsync(userFind.Id);
        result.AccessTokenExpireAt = DateTime.UtcNow.AddMinutes(jwtConfig.ExpireInMinutes); // Set expiration for access token



        // Step 9: Check for active refresh tokens and assign or generate new one
        if (userFind.RefreshTokens.Any(rt => rt.IsActive))
        {

            // If there is an active refresh token, use it
            var activeRefreshToken = userFind.RefreshTokens.FirstOrDefault(rt => rt.IsActive);
            result.RefreshToken = activeRefreshToken.Token;
            result.RefreshTokenExpireAt = activeRefreshToken.ExpirationDate;
        }
        else
        {

            // Otherwise, generate a new refresh token
            var newToken = await GenerateNewRefreshTokenAsync(userFind.Id);
            result.RefreshToken = newToken.Token;
            result.RefreshTokenExpireAt = newToken.ExpirationDate;
        }

        // Step 10: Update user details (e.g., activate user, etc.)
        await userRepository.UpdateAsync(userFind);

        // Step 11: Return the authentication result with tokens
        return result;
    }

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand refreshTokenCommand)
    {
       
        var user = await (await userRepository.WithDetailsAsync(x => x.RefreshTokens))
            .FirstOrDefaultAsync(c => c.RefreshTokens.Any(x => x.Token == refreshTokenCommand.RefreshToken))
            ?? throw new UserFriendlyException(localizer["user_not_found"], "404");

        var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshTokenCommand.RefreshToken);

        if (refreshToken is null || !refreshToken.IsActive)
        {
            throw new UserFriendlyException(localizer["invalid_refresh_token"], "400");
        }

   
        refreshToken.Revoke();

      
        var result = ObjectMapper.Map<User, AuthResult>(user);

    
        var roles = await userManager.GetRolesAsync(user);
        result.Role = roles.FirstOrDefault();
       
        result.AccessToken = await GenerateTokenAsync(user.Id);
        result.AccessTokenExpireAt = DateTime.Now.AddMinutes(jwtConfig.ExpireInMinutes);

    
        var newToken = await GenerateNewRefreshTokenAsync(user.Id);
        result.RefreshToken = newToken.Token;
        result.RefreshTokenExpireAt = newToken.ExpirationDate;

        return result;
    }
    public async Task<string> GenerateTokenAsync(Guid userId, List<Claim>? claims = null)
    {
        var user = await userRepository.GetAsync(userId);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(jwtConfig.ExpireInMinutes);

        claims ??= [];
        claims.AddRange(
        [
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (ClaimTypes.MobilePhone, user.PhoneNumber),
            new (ClaimTypes.Role,(await userManager.GetRolesAsync(user))[0])
        ]);

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken?> GenerateNewRefreshTokenAsync(Guid userId)
    {
        var user = await userRepository.GetAsync(userId);
        var refreshToken = new RefreshToken(DateTime.UtcNow.AddDays(10));
        refreshToken.UserId = userId;

        await refreshTokenRepository.InsertAsync(refreshToken);
        //user.AddRefreshToken(refreshToken);

        //await userRepository.UpdateAsync(user);

        return refreshToken;
    }
    public async Task<GetProfileResult> GetProfileAsync()
    {
        var userId = currentUser.GetId();

        var user = await userRepository.GetAsync(userId);
        var dto = ObjectMapper.Map<User, GetProfileResult>(user);

        var roles = currentUser.Roles;

       
        dto.UnreadNotificationsCount = await notificationRepository.CountAsync(x => x.UserId == userId && !x.IsRead);

        var devices = await userDeviceRepository.GetListAsync(x => x.UserId == userId);
        dto.IsNotificationsEnabled = devices.Any(x => x.IsActive);

        if (roles.Contains("DOCTOR"))
        {
            var doctor = await (await doctorRepository.WithDetailsAsync(d => d.Qualifications, d => d.User, d => d.Specialty))
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor != null)
            {
                var doctorDto = ObjectMapper.Map<Doctor, DoctorDto>(doctor);
                dto.SpecialtyId = doctor.SpecialtyId;
                dto.SpecialtyNameAr = doctor.Specialty.NameAr;
                dto.SpecialtyNameEn = doctor.Specialty.NameEn;
                dto.YearsOfExperience = doctor.YearsOfExperience;
                dto.DescriptionAr = doctor.DescriptionAr;
                dto.DescriptionEn = doctor.DescriptionEn;
                dto.CanPublishBlogs = doctor.CanPublishBlogs;
                dto.Qualifications = doctorDto.Qualifications;
            }
        }
        else if (roles.Contains("LAB"))
        {
            var labQuery = await labRepository.GetQueryableAsync();
            var lab = await labQuery
                .Include(l => l.Branches)
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (lab != null)
            {
                dto.IsActive = lab.IsActive;
                if (lab.Branches != null && lab.Branches.Any())
                {
                    var primaryBranches = lab.Branches.Where(b => b.IsPrimary).ToList();
                    dto.Branches = ObjectMapper.Map<List<LabBranch>, List<LabBranchDto>>(primaryBranches);
                }
            }
        }

        dto.Role = roles.FirstOrDefault();
        return dto;
    }
    public async Task<UpdateProfileResult> UpdateProfileAsync(UpdateProfileCommand command)
    {
        var userId = currentUser.GetId();

        // Fetch user with Otp details
        // English comment: Use await directly instead of .Result to avoid deadlocks
        var user = await (await userRepository.WithDetailsAsync(x => x.Otp))
                                        .FirstOrDefaultAsync(u => u.Id == userId);

        var roles = currentUser.Roles;

        if (user == null)
            throw new UserFriendlyException(localizer["user_not_found"], "404");

        // 1. Update general info immediately (Name, Gender)
        string newGender = user.Gender;
        if (command.UserGender.HasValue)
        {
            newGender = command.UserGender.Value.ToString().ToLowerInvariant();
        }

        // Keep original email for now in the main entity
        user.UpdateProfile(command.Name, user.Email, newGender);

        // 2. Image Processing (Immediate)
        if (command.ProfileImage != null && command.ProfileImage.Length > 0)
        {
            var url = await storageService.Upload(command.ProfileImage);

            // Handle URL formatting
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                user.ProfileImage = uri.PathAndQuery;
            else
                user.ProfileImage = url;
        }
        else if (command.DeleteProfileImage)
        {
            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                await storageService.Delete(user.ProfileImage);
                user.ProfileImage = null;
            }
        }

        // 3. Role Specific Logic (Doctor/Lab) - Immediate
        if (roles.Contains("DOCTOR"))
        {
            var doctor = await doctorRepository.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor != null)
            {
                doctor.SpecialtyId = command.SpecialtyId ?? doctor.SpecialtyId;
                doctor.YearsOfExperience = command.YearsOfExperience ?? doctor.YearsOfExperience;
                doctor.DescriptionAr = command.DescriptionAr ?? doctor.DescriptionAr;
                doctor.DescriptionEn = command.DescriptionEn ?? doctor.DescriptionEn;
                // Add your qualifications logic here...
                await doctorRepository.UpdateAsync(doctor);
            }
        }
        else if (roles.Contains("LAB"))
        {
            var lab = await labRepository.FirstOrDefaultAsync(l => l.UserId == userId);
            if (lab != null)
            {
                lab.IsActive = command.IsActive ?? lab.IsActive;
                await labRepository.UpdateAsync(lab);
            }
        }

        // 4. Check for Sensitive Data Changes (Email / Phone)
        bool emailChanged = !string.IsNullOrEmpty(command.Email) && user.Email != command.Email;
        bool phoneChanged = !string.IsNullOrEmpty(command.PhoneNumber) && user.PhoneNumber != command.PhoneNumber;

        if (emailChanged || phoneChanged)
        {
            // Validation for uniqueness
            if (emailChanged && await userRepository.AnyAsync(x => x.Email == command.Email && x.Id != userId))
                throw new UserFriendlyException(localizer["email_already_exist"], "400");

            if (phoneChanged && await userRepository.AnyAsync(x => x.PhoneNumber == command.PhoneNumber && x.Id != userId))
                throw new UserFriendlyException(localizer["phone_number_already_exist"], "400");

            // Clear old OTPs
            if (user.Otp != null)
                await otpRepository.HardDeleteAsync(user.Otp, true);

            // Create new OTP and store "Pending" values
            var newOtp = new Otp
            {
                UserId = user.Id,
                PhoneCode = GenerateOtp.GetOtp(),
                PhoneCodeExpireAt = DateTime.UtcNow.AddMinutes(5),
                Type = OtpTypeEnum.ConfirmPhone,
                NewEmail = command.Email,
                NewPhone = command.PhoneNumber // Based on your Otp Entity field
            };

            await otpRepository.InsertAsync(newOtp, true);

            // Save current user changes (Name/Image) before returning
            await userRepository.UpdateAsync(user);

            return new UpdateProfileResult
            {
                Name = user.Name,
                Email = user.Email, // Old email
                PhoneNumber = user.PhoneNumber, // Old phone
                PhoneCode = "1111", // Debugging/Helper code
                OtpTtlInMinutes = 5,
                RequiresVerification = true
            };
        }

        // 5. Final Save and Return for normal updates
        await userRepository.UpdateAsync(user);

        return new UpdateProfileResult
        {
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RequiresVerification = false
        };
    }
    public async Task<bool> VerifyUpdateOtpAsync(VerifyUpdateOtpCommand command)
    {
        var userId = currentUser.GetId();
        var user = await userRepository.WithDetailsAsync(x => x.Otp)
                                       .Result
                                       .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null || user.Otp == null)
            throw new UserFriendlyException(localizer["otp_not_sent"], "400");

        if (user.Otp.PhoneCode != command.Otp)
            throw new UserFriendlyException(localizer["invalid_otp"], "400");

        if (user.Otp.PhoneCodeExpireAt < DateTime.UtcNow)
            throw new UserFriendlyException(localizer["otp_expired"], "400");


        if (!string.IsNullOrEmpty(user.Otp.NewEmail))
        {
            user.SetEmail(user.Otp.NewEmail);
        }

        if (!string.IsNullOrEmpty(user.Otp.NewPhone))
        {
            user.SetPhoneNumber(user.Otp.NewPhone);
        }

        await userRepository.UpdateAsync(user);

    
        await otpRepository.HardDeleteAsync(user.Otp, true);

        return true;
    }
    public async Task LogoutAsync()
    {
        var userId = currentUser.GetId();

      
        var user = await (await userRepository.WithDetailsAsync(x => x.RefreshTokens))
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null)
        {
           
            user.RefreshTokens.ToList().ForEach(rt => rt.Revoke());

          
            var devices = await userDeviceRepository.GetListAsync(x => x.UserId == userId);
            foreach (var device in devices)
            {
                await userDeviceRepository.DeleteAsync(device);
            }

            await userRepository.UpdateAsync(user);
        }
    }


    public async Task<SendOtpResult> SignInWithEmailAndSendOtpAsync(SignInWithEmailDto input)
    {
        // 1. Find user by email
        var user = await userRepository.WithDetailsAsync(x => x.Otp)
            .Result
            .FirstOrDefaultAsync(x => x.Email == input.Email);

        if (user == null)
        {
            // For security, do not reveal if user exists or not, but for now we follow existing pattern or generic error
            throw new UserFriendlyException(localizer["invalid_username_or_password"], "400");
        }

        // 2. Validate Password
        if (!await userManager.CheckPasswordAsync(user, input.Password))
        {
            throw new UserFriendlyException(localizer["invalid_username_or_password"], "400");
        }

        // 3. Remove existing OTP
        if (user.Otp != null)
        {
            await otpRepository.HardDeleteAsync(user.Otp, true);
        }

        // 4. Generate new OTP
        var newOtp = new Otp
        {
            UserId = user.Id,
            PhoneCode = GenerateOtp.GetOtp(),
            PhoneCodeExpireAt = DateTime.UtcNow.AddMinutes(1),
            Type = OtpTypeEnum.ConfirmEmail
        };

        // 5. Save OTP
        await otpRepository.InsertAsync(newOtp, true);

        // 6. Return result (In a real scenario, this would trigger an SMS)
        // Note: The requirement is to send OTP to the phone of that user.
        // Assuming SendOtpResult is sufficient for now, or if we need to call an SMS provider here.
        // Since SendOTPAsync just returns the code, we mimic that behavior.

        return new SendOtpResult
        {
            ExpiryTimeByMinute = newOtp.PhoneCodeExpireAt,
            OtpCode = newOtp.PhoneCode,
            Email = user.Email
        };
    }
}


