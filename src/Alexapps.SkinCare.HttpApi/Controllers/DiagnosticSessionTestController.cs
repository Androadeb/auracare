using Alexapps.SkinCare.Dtos.Common; // تأكد من استدعاء الـ namespace الخاص بالـ PagedResult
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers
{
    [RemoteService(Name = "DiagnosticSessionTest")]
    public class DiagnosticSessionTestController : SkinCareController
    {
        private readonly IDiagnosticSessionTestAppService _service;

        public DiagnosticSessionTestController(IDiagnosticSessionTestAppService service)
        {
            _service = service;
        }

        // --- CLIENT SECTION ---

        [HttpGet(ApiRoutes.ClientDiagnosticSessionTests.NearbyLabs)]
        public async Task<List<LabForTestResponseDto>> GetNearbyLabsForTestAsync([FromQuery] GetNearbyLabsForTestQuery input)
        {
            return await _service.GetNearbyLabsForTestAsync(input);
        }

        [HttpGet(ApiRoutes.ClientDiagnosticSessionTests.AvailableSlots)]
        public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync([FromQuery] GetAvailableTimeSlotsInput input)
        {
            return await _service.GetAvailableTimeSlotsAsync(input);
        }

        [HttpPost(ApiRoutes.ClientDiagnosticSessionTests.Base + "{id}/confirm-booking")]
        public async Task<BookingConfirmationDto> ConfirmBookingAsync(Guid id, [FromBody] ConfirmLabBookingDto input)
        {
            return await _service.ConfirmBookingAsync(id, input);
        }

        // --- LAB SECTION ---

      
        [HttpGet(ApiRoutes.LabDiagnosticSessionTests.Orders)]
        public async Task<LabOrdersPagedResultDto<LabOrderResponseDto>> GetLabOrdersAsync([FromQuery] GetOrderListRequestDto input)
        {
            return await _service.GetLabOrdersAsync(input);
        }

        [HttpGet(ApiRoutes.LabDiagnosticSessionTests.Details)]
        public async Task<LabOrderDetailDto> GetLabDetailsAsync(Guid id)
        {
            return await _service.GetLabDetailsAsync(id);
        }

        [HttpPut(ApiRoutes.LabDiagnosticSessionTests.Base + "{id}/status")]
        public async Task UpdateStatusAsync(Guid id, [FromBody] UpdateTestStatusDto input)
        {
            await _service.UpdateStatusAsync(id, input);
        }

        [HttpPost(ApiRoutes.LabDiagnosticSessionTests.UploadResult)]
        [Consumes("multipart/form-data")]
        public async Task<UploadResultResponseDto> UploadResultAsync(Guid id, [FromForm] UploadTestResultDto input)
        {
            
            return await _service.UploadResultAsync(id, input); 
        }

        // --- DOCTOR SECTION ---


        [HttpGet(ApiRoutes.DoctorDiagnosticSessionTests.Orders)]
        public async Task<PagedResultWithMetadata<DoctorOrderResponseDto>> GetDoctorOrdersAsync([FromQuery] GetOrderListResponse input)
        {
            return await _service.GetDoctorOrdersAsync(input);
        }

        [HttpGet(ApiRoutes.DoctorDiagnosticSessionTests.Details)]
        public async Task<DoctorOrderDetailDto> GetDoctorDetailsAsync(Guid id)
        {
            return await _service.GetDoctorDetailsAsync(id);
        }
    }
}