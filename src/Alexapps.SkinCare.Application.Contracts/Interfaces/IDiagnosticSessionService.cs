using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IDiagnosticSessionService : IApplicationService
    {
    
        Task<DiagnosticSessionDto> CreateAsync(CreateDiagnosticSessionDto input);
        Task<DiagnosticSessionDto> GetAsync(Guid id);
        Task<PagedResultWithMetadata<DiagnosticSessionDto>> GetListAsync(GetDiagnosticSessionListDto input);
        Task<DiagnosticSessionMessageDto> SendMessageAsync(CreateDiagnosticSessionMessageDto input);
        Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> GetMessagesAsync(GetDiagnosticSessionMessagesDto input);

    
        Task<PagedResultWithMetadata<DiagnosticSessionDto>> DoctorGetListAsync(GetDiagnosticSessionListDto input);
        Task<DiagnosticSessionDto> DoctorGetAsync(Guid id);
        Task<DiagnosticSessionMessageDto> DoctorSendMessageAsync(CreateDiagnosticSessionMessageDto input);
        Task<DiagnosticSessionMessageDto> DoctorSendTestAsync(CreateDiagnosticSessionTestDto input);
        Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> DoctorGetMessagesAsync(GetDiagnosticSessionMessagesDto input);
        Task<DiagnosticSessionMessageTestResultDto> DoctorSendTestResultAsync(SendTestResultDto input);

        Task<DiagnosticSessionMessageTreatmentPlanDto> AddTreatmentPlanItemAsync(AddIndividualTreatmentPlanDto input);


        Task DeleteTreatmentPlanItemAsync(Guid id);
        Task<List<DiagnosticSessionMessageTreatmentPlanDto>> GetAddedTreatmentPlanItemsAsync(Guid sessionId);
        Task<DiagnosticSessionMessageDto> DoctorSendTreatmentPlanAsync(Guid sessionId);
        Task<BookTreatmentPlanResultDto> BookTreatmentPlanAsync(BookTreatmentPlanDto input);
        Task<DiagnosticSessionFullDetailsDto> GetSessionFullDetailsAsync(Guid sessionId, GetDiagnosticSessionDetailsDto input);
    }
}