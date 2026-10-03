using Alexapps.SkinCare.Business.Client.Profile.Results;
using Alexapps.SkinCare.Business.OTP.Commands.SendOtp;
using Alexapps.SkinCare.Business.OTP.Commands.VerifyOtp;
using Alexapps.SkinCare.Business.OTP.Results;
using Alexapps.SkinCare.Dtos.Auth.Commands.RefreshToken;
using Alexapps.SkinCare.Dtos.Auth.Commands.Register;
using Alexapps.SkinCare.Dtos.Auth.Results.Login;
using Alexapps.SkinCare.Dtos.Auth.Results.Register;
using Alexapps.SkinCare.Entities.Users;
using AutoMapper;
using Alexapps.SkinCare.MappingProfiles.Actions;

namespace Alexapps.SkinCare.MappingProfiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, AuthResult>();
        CreateMap<User, RegisterResult>();
        CreateMap<User, UpdateProfileResult>();
        CreateMap<User, GetProfileResult>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.ProfileImage))
            .AfterMap<UserImageUrlMappingAction>();
        CreateMap<User, SendOtpResult>();

        CreateMap<RegisterCommand, User>();
        CreateMap<VerifyOtpCommand, User>();
        CreateMap<SendOtpCommand, User>();
        CreateMap<RefreshTokenCommand, User>();
    }
}
