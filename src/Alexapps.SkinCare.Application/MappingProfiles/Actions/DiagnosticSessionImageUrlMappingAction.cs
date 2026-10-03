using Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries;
using Alexapps.SkinCare.Entities.Consultations;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions;

public class DiagnosticSessionImageUrlMappingAction : IMappingAction<DiagnosticSession, DiagnosticSessionDto>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public DiagnosticSessionImageUrlMappingAction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Process(DiagnosticSession source, DiagnosticSessionDto destination, ResolutionContext context)
    {
        if (destination.Images != null && destination.Images.Any())
        {
            var baseUrl = _configuration["App:SelfUrl"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                destination.Images = destination.Images.Select(img =>
                {
                    if (string.IsNullOrEmpty(img)) return img;
                    if (img.StartsWith("http", System.StringComparison.OrdinalIgnoreCase)) return img;
                    
                    var image = img.TrimStart('/');
                    return $"{baseUrl.TrimEnd('/')}/{image}";
                }).ToList();
            }
        }
    }
}
