using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Queries;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace Alexapps.SkinCare.Controllers
{
    [RemoteService]
    public class DoctorDiagnosticSessionController : SkinCareController
    {
        private readonly IDiagnosticSessionService _diagnosticSessionService;

        public DoctorDiagnosticSessionController(IDiagnosticSessionService diagnosticSessionService)
        {
            _diagnosticSessionService = diagnosticSessionService;
        }

        [HttpGet(ApiRoutes.DoctorDiagnosticSessions.Base)]
        public Task<PagedResultWithMetadata<DiagnosticSessionDto>> GetListAsync([FromQuery] GetDiagnosticSessionListDto input)
        {
            return _diagnosticSessionService.DoctorGetListAsync(input);
        }


        [HttpGet(ApiRoutes.DoctorDiagnosticSessions.Single)]
        public Task<DiagnosticSessionDto> GetAsync(Guid id)
        {
            return _diagnosticSessionService.DoctorGetAsync(id);
        }

        [HttpPost(ApiRoutes.DoctorDiagnosticSessions.Messages)]
        public Task<DiagnosticSessionMessageDto> SendMessageAsync([FromForm] CreateDiagnosticSessionMessageDto input)
        {
            return _diagnosticSessionService.DoctorSendMessageAsync(input);
        }

        [HttpPost(ApiRoutes.DoctorDiagnosticSessions.Tests)]
        public Task<DiagnosticSessionMessageDto> SendTestAsync([FromBody] CreateDiagnosticSessionTestDto input)
        {
            return _diagnosticSessionService.DoctorSendTestAsync(input);
        }

       
        
        [HttpGet(ApiRoutes.DoctorDiagnosticSessions.SessionMessages)]
        public Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> GetMessagesAsync(Guid id, [FromQuery] GetDiagnosticSessionMessagesDto input)
        {
            input.DiagnosticSessionId = id;
            return _diagnosticSessionService.DoctorGetMessagesAsync(input);
        }
        [HttpPost(ApiRoutes.DoctorDiagnosticSessions.AddTreatmentPlanItem)]
        public Task<DiagnosticSessionMessageTreatmentPlanDto> AddTreatmentPlanItemAsync([FromBody] AddIndividualTreatmentPlanDto input)
        {
            return _diagnosticSessionService.AddTreatmentPlanItemAsync(input);
        }
        [HttpDelete(ApiRoutes.DoctorDiagnosticSessions.DeleteTreatmentPlanItem)]
        public Task DeleteTreatmentPlanItemAsync(Guid id)
        {
            return _diagnosticSessionService.DeleteTreatmentPlanItemAsync(id);
        }

        [HttpGet(ApiRoutes.DoctorDiagnosticSessions.GetAddedTreatmentPlanItems)]
        public Task<List<DiagnosticSessionMessageTreatmentPlanDto>> GetAddedTreatmentPlanItemsAsync(Guid sessionId)
        {
            return _diagnosticSessionService.GetAddedTreatmentPlanItemsAsync(sessionId);
        }
        [HttpPost(ApiRoutes.DoctorDiagnosticSessions.SendTreatmentPlan)]
        public Task<DiagnosticSessionMessageDto> SendTreatmentPlanAsync(Guid id) 
        {
            return _diagnosticSessionService.DoctorSendTreatmentPlanAsync(id);
        }
    }
}
