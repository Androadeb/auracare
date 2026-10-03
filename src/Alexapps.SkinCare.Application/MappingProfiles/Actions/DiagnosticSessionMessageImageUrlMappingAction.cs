using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Entities.Consultations;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Alexapps.SkinCare.MappingProfiles.Actions;

public class DiagnosticSessionMessageImageUrlMappingAction : IMappingAction<DiagnosticSessionMessage, DiagnosticSessionMessageDto>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public DiagnosticSessionMessageImageUrlMappingAction(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Process(DiagnosticSessionMessage source, DiagnosticSessionMessageDto destination, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(destination.FileUrl))
        {
            var baseUrl = _configuration["App:SelfUrl"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                if (!destination.FileUrl.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                {
                    var f = destination.FileUrl.TrimStart('/');
                    destination.FileUrl = $"{baseUrl.TrimEnd('/')}/{f}";
                }
            }
        }
    }
}
