using Alexapps.SkinCare.Business.Client.Profile.Results;
using Alexapps.SkinCare.Entities.Users;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions;

public class UserImageUrlMappingAction : IMappingAction<User, GetProfileResult>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public UserImageUrlMappingAction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Process(User source, GetProfileResult destination, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.ProfileImage))
        {
            var baseUrl = _configuration["App:SelfUrl"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                var image = source.ProfileImage.TrimStart('/');
                destination.ProfilePictureUrl = $"{baseUrl.TrimEnd('/')}/{image}";
            }
            else
            {
                destination.ProfilePictureUrl = source.ProfileImage;
            }
        }
    }
}
