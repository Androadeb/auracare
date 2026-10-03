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

namespace Alexapps.SkinCare.MappingProfiles
{
    public class UserMapper : Profile
    {
        public UserMapper() {

            CreateMap<RegisterCommand, User>();
            CreateMap<VerifyOtpCommand, User>();
            CreateMap<SendOtpCommand, User>();
            CreateMap<RefreshTokenCommand, User>();

            CreateMap<User, RegisterResult>();
            CreateMap<User, AuthResult>();
            CreateMap<User, SendOtpResult>();
            CreateMap<User, GetProfileResult>();

        }
    }
}


