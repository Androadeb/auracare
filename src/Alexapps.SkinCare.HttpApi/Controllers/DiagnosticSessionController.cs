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
    public class DiagnosticSessionController : SkinCareController
    {
        private readonly IDiagnosticSessionService _diagnosticSessionService;

        public DiagnosticSessionController(IDiagnosticSessionService diagnosticSessionService)
        {
            _diagnosticSessionService = diagnosticSessionService;
        }

        [HttpPost(ApiRoutes.DiagnosticSessions.Base)]
        public Task<DiagnosticSessionDto> CreateAsync([FromForm] CreateDiagnosticSessionDto input)
        {
            return _diagnosticSessionService.CreateAsync(input);
        }

        [HttpGet(ApiRoutes.DiagnosticSessions.Single)]
        public Task<DiagnosticSessionDto> GetAsync(Guid id)
        {
            return _diagnosticSessionService.GetAsync(id);
        }

        [HttpGet(ApiRoutes.DiagnosticSessions.Base)]
        public Task<PagedResultWithMetadata<DiagnosticSessionDto>> GetListAsync([FromQuery] GetDiagnosticSessionListDto input)
        {
            return _diagnosticSessionService.GetListAsync(input);
        }


        [HttpPost(ApiRoutes.DiagnosticSessions.Messages)]
        public Task<DiagnosticSessionMessageDto> SendMessageAsync([FromForm] CreateDiagnosticSessionMessageDto input)
        {
            return _diagnosticSessionService.SendMessageAsync(input);
        }

        [HttpGet(ApiRoutes.DiagnosticSessions.SessionMessages)]
        public Task<PagedResultWithMetadata<DiagnosticSessionMessageDto>> GetMessagesAsync(Guid id, [FromQuery] GetDiagnosticSessionMessagesDto input)
        {
            input.DiagnosticSessionId = id;
            return _diagnosticSessionService.GetMessagesAsync(input);
        }
        [HttpGet(ApiRoutes.DiagnosticSessions.FullDetails)] 
        public Task<DiagnosticSessionFullDetailsDto> GetFullDetailsAsync(Guid id, [FromQuery] GetDiagnosticSessionDetailsDto input)
        {
            
            return _diagnosticSessionService.GetSessionFullDetailsAsync(id, input);
        }

        [HttpPost(ApiRoutes.DoctorDiagnosticSessions.SendTestResult)]
        public Task<DiagnosticSessionMessageTestResultDto> DoctorSendTestResultAsync([FromForm] SendTestResultDto input)
        {
            return _diagnosticSessionService.DoctorSendTestResultAsync(input);
        }
        [HttpPost(ApiRoutes.DiagnosticSessions.BookTreatmentPlan)]
        public async Task<BookTreatmentPlanResultDto> BookTreatmentPlanAsync([FromBody] BookTreatmentPlanDto input)
        {
          
            return await _diagnosticSessionService.BookTreatmentPlanAsync(input);
        }


    }
}
