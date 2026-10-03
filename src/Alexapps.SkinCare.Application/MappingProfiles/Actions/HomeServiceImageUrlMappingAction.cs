using Alexapps.SkinCare.Dtos.HomeServices.Results;
using Alexapps.SkinCare.Dtos.HomeServices.Queries;
using Alexapps.SkinCare.Entities.HomeServices;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions;

public class HomeServiceImageUrlMappingAction : IMappingAction<HomeService, HomeServiceDto>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public HomeServiceImageUrlMappingAction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Process(HomeService source, HomeServiceDto destination, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.Image))
        {
            var baseUrl = _configuration["App:SelfUrl"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                var image = source.Image.TrimStart('/');
                destination.Image = $"{baseUrl.TrimEnd('/')}/{image}";
            }
        }
    }
}
