using Alexapps.SkinCare.Dtos.Common; // عشان PagedResultWithMetadata
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IDiagnosticSessionTestAppService : IApplicationService
    {
        Task<List<LabForTestResponseDto>> GetNearbyLabsForTestAsync(GetNearbyLabsForTestQuery input);

        Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(GetAvailableTimeSlotsInput input);

        Task<BookingConfirmationDto> ConfirmBookingAsync(Guid id, ConfirmLabBookingDto input);

        Task<LabOrdersPagedResultDto<LabOrderResponseDto>> GetLabOrdersAsync(GetOrderListRequestDto input);

        Task<PagedResultWithMetadata<DoctorOrderResponseDto>> GetDoctorOrdersAsync(GetOrderListResponse input);

        Task UpdateStatusAsync(Guid id, UpdateTestStatusDto input);

        Task<UploadResultResponseDto> UploadResultAsync(Guid id, UploadTestResultDto input);

        Task<LabOrderDetailDto> GetLabDetailsAsync(Guid id);

        Task<DoctorOrderDetailDto> GetDoctorDetailsAsync(Guid id);
    }
}