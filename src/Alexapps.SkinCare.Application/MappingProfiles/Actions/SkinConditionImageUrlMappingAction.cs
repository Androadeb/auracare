using Alexapps.SkinCare.Dtos.SkinConditions.Results;
using Alexapps.SkinCare.Entities.SkinConditions;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions;

public class SkinConditionImageUrlMappingAction : IMappingAction<SkinCondition, SkinConditionDto>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public SkinConditionImageUrlMappingAction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Process(SkinCondition source, SkinConditionDto destination, ResolutionContext context)
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
