
using Volo.Abp.Application.Dtos;


namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries
{
    public class GetDiagnosticSessionListDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Filter { get; set; }
    }
}
