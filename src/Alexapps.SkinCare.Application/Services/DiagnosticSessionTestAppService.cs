using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Commands;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Queries;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using AutoMapper.Internal.Mappers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Validation;

namespace Alexapps.SkinCare.Services
{
    [Authorize]
    public class DiagnosticSessionTestAppService : ApplicationService, IDiagnosticSessionTestAppService
    {
        private readonly IRepository<Lab, Guid> _labRepository;
        private readonly IRepository<LabMedicalTest, Guid> _labMedicalTestRepository;
        private readonly IRepository<DiagnosticSessionTest, Guid> _sessionTestRepository;
        private readonly IRepository<LabSchedule, Guid> _labScheduleRepository;
        private readonly IValidator<GetNearbyLabsForTestQuery> _validator;
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<LabBranch, Guid> _labBranchRepository;

        public DiagnosticSessionTestAppService(
            IRepository<Lab, Guid> labRepository,
            IRepository<LabBranch, Guid> labBranchRepository,
            IRepository<LabMedicalTest, Guid> labMedicalTestRepository,
            IRepository<DiagnosticSessionTest, Guid> sessionTestRepository,
            IRepository<LabSchedule, Guid> labScheduleRepository, 
            IRepository<Doctor, Guid> doctorRepository,
            IValidator<GetNearbyLabsForTestQuery> validator)
        {
            _labRepository = labRepository;
            _labMedicalTestRepository = labMedicalTestRepository;
            _sessionTestRepository = sessionTestRepository;
            _labScheduleRepository = labScheduleRepository; 
            _doctorRepository = doctorRepository;
            _labBranchRepository = labBranchRepository;
            _validator = validator;
        }

        public async Task<List<LabForTestResponseDto>> GetNearbyLabsForTestAsync(GetNearbyLabsForTestQuery input)
        {
            var sessionTest = await _sessionTestRepository.GetAsync(input.DiagnosticSessionTestId);

            // جلب قائمة التحاليل المتاحة في المعامل مع بيانات المعمل الأساسية
            var labMedicalTests = await _labMedicalTestRepository.WithDetailsAsync(lt => lt.Lab.User);

            var filteredLabTests = labMedicalTests
                .Where(lt => lt.MedicalTestId == sessionTest.MedicalTestId && lt.IsEnabled)
                .WhereIf(input.IsHomeService, lt => lt.ServiceScope == LabServiceScope.Both)
                .ToList();

            var response = new List<LabForTestResponseDto>();

            foreach (var lt in filteredLabTests)
            {
                // جلب فروع المعمل النشطة
                var branches = await _labBranchRepository.GetListAsync(b => b.LabId == lt.LabId && b.IsActive);

                foreach (var branch in branches)
                {
                    // حساب المسافة بين العميل وموقع الفرع
                    var distance = CalculateDistance(input.CustomerLat, input.CustomerLong, branch.Latitude, branch.Longitude);

                    response.Add(new LabForTestResponseDto
                    {
                        LabId = lt.LabId,
                        BranchId = branch.Id,
                        LabName = lt.Lab.User?.Name ?? "Unknown Lab",
                        BranchName = branch.Name,
                        FullAddress = branch.FullAddress,
                        Latitude = branch.Latitude,
                        Longitude = branch.Longitude,
                        Distance = distance,
                        OffersHomeService = lt.ServiceScope == LabServiceScope.Both,
                        LabPrice = lt.LabPrice
                    });
                }
            }

            // ترتيب المعامل حسب المسافة (الأقرب فالأبعد)
            var sortedResponse = response.OrderBy(l => l.Distance).ToList();

            // لو خدمة منزلية بنرجع أقرب واحد فقط، لو زيارة معمل بنرجع الكل مرتبين
            return input.IsHomeService ? sortedResponse.Take(1).ToList() : sortedResponse;
        }
        private async Task<bool> CheckSlotAvailabilityAsync(Guid branchId, DateTime appointmentTime)
        {
            var branch = await _labBranchRepository.GetAsync(branchId);

            // البحث عن جدول مواعيد المعمل (السعة مرتبطة بالمعمل أو بالفرع حسب تصميمك)
            var schedule = await _labScheduleRepository.FirstOrDefaultAsync(s =>
                s.LabId == branch.LabId && s.DayOfWeek == appointmentTime.DayOfWeek && s.IsOpen);

            if (schedule == null) return false;

            var startOfHour = new DateTime(appointmentTime.Year, appointmentTime.Month, appointmentTime.Day, appointmentTime.Hour, 0, 0);
            var endOfHour = startOfHour.AddHours(1);

            // عد الحجوزات الموجودة فعلياً في هذا الفرع خلال هذه الساعة
            var bookingsCount = await _sessionTestRepository.CountAsync(t =>
                t.BranchId == branchId &&
                t.AppointmentDate >= startOfHour &&
                t.AppointmentDate < endOfHour);

            return bookingsCount < schedule.CapacityPerHour;
        }

        public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(GetAvailableTimeSlotsInput input)
        {
            // ملاحظة: هنا الـ Input لازم يحتوي على BranchId بدل LabId لضمان دقة المواعيد لكل فرع
            var branch = await _labBranchRepository.GetAsync(input.BranchId);

            var schedule = await _labScheduleRepository.FirstOrDefaultAsync(s =>
                s.LabId == branch.LabId && s.DayOfWeek == input.Date.DayOfWeek && s.IsOpen);

            if (schedule == null) return new List<TimeSlotDto>();

            var slots = new List<TimeSlotDto>();
            var currentSlotTime = schedule.OpeningTime;

            while (currentSlotTime < schedule.ClosingTime)
            {
                var slotDateTime = input.Date.Date.Add(currentSlotTime);

                // التحقق من توافر الساعة في هذا الفرع بالتحديد
                var isAvailable = await CheckSlotAvailabilityAsync(input.BranchId, slotDateTime);

                slots.Add(new TimeSlotDto
                {
                    TimeLabel = slotDateTime.ToString("hh:mm tt"),
                    ActualDateTime = slotDateTime,
                    IsAvailable = isAvailable
                });

                currentSlotTime = currentSlotTime.Add(TimeSpan.FromHours(1));
            }

            return slots;
        }
        public async Task<BookingConfirmationDto> ConfirmBookingAsync(Guid id, ConfirmLabBookingDto input)
        {
            var sessionTest = await _sessionTestRepository.GetAsync(id);

            var branch = await _labBranchRepository.GetAsync(input.BranchId);
            if (branch == null) throw new UserFriendlyException("Selected branch not found.");

       
            if (sessionTest.BranchId == input.BranchId)
            {
                throw new UserFriendlyException("You have already sent this test request to this branch.");
            }

         
            var isStillAvailable = await CheckSlotAvailabilityAsync(input.BranchId, input.AppointmentDate);
            if (!isStillAvailable)
            {
                throw new UserFriendlyException("Sorry, this time slot is no longer available in this branch.");
            }

       
            sessionTest.LabId = branch.LabId;
            sessionTest.BranchId = input.BranchId;
            sessionTest.AppointmentDate = input.AppointmentDate;
            sessionTest.ServiceType = input.ServiceType;
            sessionTest.Status = DiagnosticTestStatus.Requested;
            sessionTest.IsRead = false;

           
            sessionTest.IsPaid = true;

            sessionTest.GenerateBookingCode();

          
            if (input.ServiceType == LabServiceType.HomeSampleCollection)
            {
                if (input.Latitude.HasValue) sessionTest.Latitude = input.Latitude;
                if (input.Longitude.HasValue) sessionTest.Longitude = input.Longitude;
                if (!string.IsNullOrEmpty(input.DetailedAddress))
                    sessionTest.DetailedAddress = input.DetailedAddress;
            }

            await _sessionTestRepository.UpdateAsync(sessionTest);

            return new BookingConfirmationDto
            {
                BookingCode = sessionTest.BookingCode
            };
        }
        public async Task<LabOrdersPagedResultDto<LabOrderResponseDto>> GetLabOrdersAsync(GetOrderListRequestDto input)
        {
            // 1. Identify current lab
            var currentUserId = CurrentUser.Id;
            var lab = await _labRepository.FirstOrDefaultAsync(l => l.UserId == currentUserId);
            if (lab == null) throw new UserFriendlyException("Unauthorized access: Lab profile not found.");

            // 2. Build Base Queryable
            var query = await _sessionTestRepository.GetQueryableAsync();

            query = query
                .Include(t => t.MedicalTest)
                .Include(t => t.DiagnosticSession.User)
                .Include(t => t.DiagnosticSession.Doctor.User)
                .Where(t => t.LabId == lab.Id && t.Status >= DiagnosticTestStatus.Requested);

            // 3. Apply Common Filters
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(t =>
                    t.DiagnosticSession.User.Name.Contains(input.Filter) ||
                    t.BookingCode.Contains(input.Filter) ||
                    (t.MedicalTest != null && (t.MedicalTest.NameEn.Contains(input.Filter) || t.MedicalTest.NameAr.Contains(input.Filter))));
            }

            if (input.RequestDate.HasValue)
            {
                var targetDate = input.RequestDate.Value.Date;
                query = query.Where(t => t.CreationTime.Date == targetDate);
            }

            if (input.ServiceType.HasValue)
            {
                query = query.Where(t => t.ServiceType == input.ServiceType.Value);
            }

            // 4. Calculate Status Counts (قبل فلترة الـ Status المختار)
            var statusCounts = await AsyncExecuter.ToListAsync(
                query.GroupBy(t => t.Status)
                     .Select(g => new { Status = g.Key, Count = g.Count() })
            );

            int allCount = statusCounts.Sum(x => x.Count);
            int requestedCount = statusCounts.FirstOrDefault(x => x.Status == DiagnosticTestStatus.Requested)?.Count ?? 0;
            int inProgressCount = statusCounts.FirstOrDefault(x => x.Status == DiagnosticTestStatus.InProgress)?.Count ?? 0;
            int sampleCollectedCount = statusCounts.FirstOrDefault(x => x.Status == DiagnosticTestStatus.SampleCollected)?.Count ?? 0;
            int resultReadyCount = statusCounts.FirstOrDefault(x => x.Status == DiagnosticTestStatus.ResultReady)?.Count ?? 0;

            // 5. Apply Status Filter
            if (input.Status.HasValue)
            {
                query = query.Where(t => t.Status == input.Status.Value);
            }

            // 6. Count and Paginate
            var totalCount = await AsyncExecuter.CountAsync(query);

            var list = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(t => t.CreationTime)
                     .Skip((input.Page - 1) * input.Limit)
                     .Take(input.Limit)
            );

            // 7. Map and Return using our custom PagedResult
            var dtos = ObjectMapper.Map<List<DiagnosticSessionTest>, List<LabOrderResponseDto>>(list);

            return new LabOrdersPagedResultDto<LabOrderResponseDto>(
     dtos,
     totalCount,
     new LabOrdersMetadata
     {
         AllCount = allCount,
         RequestedCount = requestedCount,
         InProgressCount = inProgressCount,
         SampleCollectedCount = sampleCollectedCount,
         ResultReadyCount = resultReadyCount
     }
 );
        }
        public async Task UpdateStatusAsync(Guid id, UpdateTestStatusDto input)
        {
            // 1. Fetch the test record
            var sessionTest = await _sessionTestRepository.GetAsync(id);

            // 2. Update only the status
            sessionTest.Status = input.NewStatus;

            // 3. Save changes
            await _sessionTestRepository.UpdateAsync(sessionTest);
        }
        public async Task<PagedResultWithMetadata<DoctorOrderResponseDto>> GetDoctorOrdersAsync(GetOrderListResponse input)
        {
            // 1. Identify Doctor
            var currentUserId = CurrentUser.Id;
            var doctor = await _doctorRepository.FirstOrDefaultAsync(d => d.UserId == currentUserId);
            if (doctor == null) throw new UserFriendlyException("Doctor profile not found.");

            // 2. Build Queryable
            var query = await _sessionTestRepository.GetQueryableAsync();

            query = query
                .Include(t => t.MedicalTest)
                .Include(t => t.DiagnosticSession.User)
                .Include(t => t.Lab.User)
                .Where(t => t.DiagnosticSession.DoctorId == doctor.Id &&
                            t.LabId != null &&
                            t.Status >= DiagnosticTestStatus.Requested);

            // 3. Apply Filter (البحث باسم المريض أو اسم المعمل)
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(t =>
                    t.DiagnosticSession.User.Name.Contains(input.Filter) ||
                    t.Lab.User.Name.Contains(input.Filter) ||
                    t.MedicalTest.NameEn.Contains(input.Filter));
            }

            // 4. Count and Paginate
            var totalCount = await AsyncExecuter.CountAsync(query);

            var list = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(t => t.CreationTime)
                     .Skip((input.Page - 1) * input.Limit)
                     .Take(input.Limit)
            );

            var dtos = ObjectMapper.Map<List<DiagnosticSessionTest>, List<DoctorOrderResponseDto>>(list);
            return new PagedResultWithMetadata<DoctorOrderResponseDto>(dtos, input.Page, input.Limit, totalCount);
        }
        public async Task<LabOrderDetailDto> GetLabDetailsAsync(Guid id)
        {
            // استخدام WithDetailsAsync لضمان تحميل كل البيانات المطلوبة للـ Mapper
            var queryable = await _sessionTestRepository.WithDetailsAsync(
                t => t.MedicalTest,
                t => t.DiagnosticSession.User,
                t => t.DiagnosticSession.Doctor.User,
                t => t.DiagnosticSession.Doctor.Specialty
            );

            var test = queryable.FirstOrDefault(x => x.Id == id);

            if (test == null) throw new UserFriendlyException("Order not found");

            // هنا نرجع النوع LabOrderDetailDto
            return ObjectMapper.Map<DiagnosticSessionTest, LabOrderDetailDto>(test);
        }

        // 2. تفاصيل الدكتور (View image_c70e1a)
        public async Task<DoctorOrderDetailDto> GetDoctorDetailsAsync(Guid id)
        {
            var queryable = await _sessionTestRepository.WithDetailsAsync(
         t => t.MedicalTest,
         t => t.SampleType,
         t => t.Lab.User, 
         t => t.DiagnosticSession.User,
         t => t.MedicalTest.TestCategory 
     );

            var test = queryable.FirstOrDefault(x => x.Id == id);

            if (test == null) throw new UserFriendlyException("Order not found");

            // هنا نرجع النوع DoctorOrderDetailDto
            return ObjectMapper.Map<DiagnosticSessionTest, DoctorOrderDetailDto>(test);
        }
        public async Task<UploadResultResponseDto> UploadResultAsync(Guid id, UploadTestResultDto input)
        {
            if (input.File == null || input.File.Length == 0)
                throw new UserFriendlyException("Please upload a valid file.");

            var sessionTest = await _sessionTestRepository.GetAsync(id);

            // 1. Setup storage directory
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "results");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // 2. Generate unique file name and path
            var extension = Path.GetExtension(input.File.FileName);
            var originalFileName = Path.GetFileName(input.File.FileName);
            var uniqueFileName = $"Result_{id}_{Guid.NewGuid()}{extension}";
            var physicalPath = Path.Combine(uploadsFolder, uniqueFileName);

            // 3. Save physical file
            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await input.File.CopyToAsync(stream);
            }

            // 4. Update the fields
            sessionTest.ResultFileUrl = $"/uploads/results/{uniqueFileName}";
            sessionTest.ResultFileName = originalFileName;
            sessionTest.Status = DiagnosticTestStatus.ResultReady;

            await _sessionTestRepository.UpdateAsync(sessionTest);

            // 5. ارجاع البيانات الجديدة
            return new UploadResultResponseDto
            {
                ResultFileUrl = sessionTest.ResultFileUrl,
                ResultFileName = sessionTest.ResultFileName
            };
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var r = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return r * c;
        }

        private double ToRadians(double deg) => deg * (Math.PI / 180);
    }
}
