
using Alexapps.SkinCare.Dtos.Blogs.Commands;
using Alexapps.SkinCare.Dtos.Blogs.Results;
using Alexapps.SkinCare.Dtos.DiagnosticSessions.Results;
using Alexapps.SkinCare.Dtos.DiagnosticSessionTest.Result;
using Alexapps.SkinCare.Dtos.Doctors.Commands;
using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.LabBranches.Commands;
using Alexapps.SkinCare.Dtos.LabBranches.Result;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Command;
using Alexapps.SkinCare.Dtos.LabSchedule.Command;
using Alexapps.SkinCare.Dtos.LabSchedule.Result;
using Alexapps.SkinCare.Dtos.Notifications;
using Alexapps.SkinCare.Entities.Consultations;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.Notifications;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.MappingProfiles.Actions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AutoMapper;

namespace Alexapps.SkinCare;

public class SkinCareApplicationAutoMapperProfile : Profile
{
    public SkinCareApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Entities.Page, Dtos.Pages.Results.PageResult>();
        CreateMap<Entities.Doctors.Specialty, Dtos.Doctors.Results.SpecialtyDto>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()));

        CreateMap<Doctor, DoctorDto>()
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.User.ProfileImage))
    .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialty != null ? src.Specialty.GetName() : null))
    .ForMember(dest => dest.Description, opt => opt.MapFrom((src, dest, destMember, context) =>
    {
        var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
        return isArabic ? src.DescriptionAr : src.DescriptionEn;
    }))
    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src =>
        Math.Round(src.Rating * 2, MidpointRounding.AwayFromZero) / 2))
    
    .ForMember(dest => dest.Qualifications, opt => opt.MapFrom(src => src.Qualifications))
    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
    .AfterMap<DoctorImageUrlMappingAction>();

        CreateMap<DoctorQualification, DoctorQualificationDto>()
      .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
    
      .ForMember(dest => dest.CertificateImageUrl, opt => opt.MapFrom(src => src.CertificateImageUrl));
        CreateMap<Entities.HomeServices.HomeService, Dtos.HomeServices.Results.HomeServiceDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom((src, dest, destMember, context) => src.DescriptionAr))
            .AfterMap((src, dest) =>
            {
                var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                dest.Description = isArabic ? src.DescriptionAr : src.DescriptionEn;

                var services = isArabic ? src.ServicesAr : src.ServicesEn;
                dest.Services = services != null ? new System.Collections.Generic.List<string>(services.Split(',', System.StringSplitOptions.RemoveEmptyEntries)) : new System.Collections.Generic.List<string>();

                var notes = isArabic ? src.NotesAr : src.NotesEn;
                dest.Notes = notes != null ? new System.Collections.Generic.List<string>(notes.Split(',', System.StringSplitOptions.RemoveEmptyEntries)) : new System.Collections.Generic.List<string>();
            })
            .AfterMap<HomeServiceImageUrlMappingAction>();

        CreateMap<Entities.HomeServices.HomeServiceProvider, Dtos.HomeServiceProviders.Results.HomeServiceProviderDto>();

        CreateMap<Entities.SkinConditions.SkinCondition, Dtos.SkinConditions.Results.SkinConditionDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GetDescription()))
            .AfterMap<SkinConditionImageUrlMappingAction>();

        CreateMap<Entities.HomeServices.HomeServiceSession, Dtos.HomeServiceSessions.Results.HomeServiceSessionDto>();

        CreateMap<Entities.Doctors.DoctorSchedule, Dtos.Doctors.Results.DoctorScheduleDto>();
        CreateMap<Entities.HomeServices.HomeServiceSchedule, Dtos.HomeServices.Results.HomeServiceScheduleDto>();
        CreateMap<Entities.Consultations.DiagnosticSession, Dtos.DiagnosticSessions.Results.DiagnosticSessionDto>()
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
             .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(x => x.ImageUrl).ToList()))
             .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null))
             .ForMember(dest => dest.ClientEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
             .ForMember(dest => dest.ClientGender, opt => opt.MapFrom(src => src.User != null ? src.User.Gender : null))
             .AfterMap<DiagnosticSessionImageUrlMappingAction>();
        CreateMap<Entities.Consultations.DiagnosticSessionMessage, Dtos.DiagnosticSessions.Results.DiagnosticSessionMessageDto>()
    .ForMember(dest => dest.MedicalTest, opt => opt.MapFrom(src => src.DiagnosticSessionTest))

    .ForMember(dest => dest.TestResult, opt => opt.MapFrom(src =>
        (src.DiagnosticSessionTest != null && !string.IsNullOrEmpty(src.DiagnosticSessionTest.ResultFileUrl))
        ? src.DiagnosticSessionTest : null))

    .ForMember(dest => dest.TreatmentPlans, opt => opt.MapFrom(src => src.DiagnosticSessionTreatmentPlans))
    .ForMember(dest => dest.IsMe, opt => opt.MapFrom((src, dest, destMember, context) => false))
    .AfterMap<DiagnosticSessionMessageImageUrlMappingAction>();
        CreateMap<Entities.MedicalTests.TestCategory, Dtos.MedicalTests.Results.TestCategoryDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()));
        CreateMap<Entities.MedicalTests.MedicalTest, Dtos.MedicalTests.Results.MedicalTestDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()));
        CreateMap<Entities.MedicalTests.SampleType, Dtos.MedicalTests.Results.SampleTypeDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GetName()));
        CreateMap<Entities.LABs.LabMedicalTest, Dtos.LabMedicalTest.Result.LabMedicalTestDto>()

.ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.MedicalTest != null ? src.MedicalTest.GetName() : null))

.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
    (src.MedicalTest != null && src.MedicalTest.TestCategory != null)
    ? src.MedicalTest.TestCategory.GetName()
    : null));



        CreateMap<Dtos.LabMedicalTest.Command.CreateLabMedicalTestDto, Entities.LABs.LabMedicalTest>();
        CreateMap<CreateLabBranchDto, LabBranch>();
        CreateMap<UpdateLabBranchDto, LabBranch>()
    .IgnoreFullAuditedObjectProperties();
        CreateMap<LabSchedule, LabScheduleResult>()
     .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.DayOfWeek.ToString()))
     .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.OpeningTime.ToString(@"hh\:mm")))
     .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ClosingTime.ToString(@"hh\:mm")));


        CreateMap<UpdateLabMedicalTestDto, LabMedicalTest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .IgnoreFullAuditedObjectProperties();

        CreateMap<LabBranch, LabForTestResponseDto>()
     .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
     .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Id))
     .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Lab.User.Name))
     .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Name)) 
     .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.FullAddress))
     .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
     .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
     .ForMember(dest => dest.Distance, opt => opt.Ignore());
        CreateMap<DiagnosticSessionTest, LabOrderResponseDto>()
            .ForMember(dest => dest.NumberOrder, opt => opt.MapFrom(src => src.BookingCode))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.DiagnosticSession.Doctor.User.Name))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.DiagnosticSession.User.Name))
            .ForMember(dest => dest.MedicalTestName, opt => opt.MapFrom(src => src.MedicalTest.NameAr))
           .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AppointmentDate))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));


        CreateMap<DiagnosticSessionTest, DoctorOrderResponseDto>()
    .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.DiagnosticSession.User.Name))
    .ForMember(dest => dest.TestType, opt => opt.MapFrom(src =>
    src.MedicalTest != null
    ? src.MedicalTest.GetName()
    : null))
    .ForMember(dest => dest.RequestedDate, opt => opt.MapFrom(src => src.CreationTime))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
     .ForMember(dest => dest.PatientImage, opt => opt.MapFrom(src => src.DiagnosticSession.User.ProfileImage));
        CreateMap<DiagnosticSessionTest, LabOrderDetailDto>()
                 // Patient Info (Automatically traverses DiagnosticSession -> User)
                 .ForMember(dest => dest.NumberOrder, opt => opt.MapFrom(src => src.BookingCode))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.DiagnosticSession.User.Name))
                .ForMember(dest => dest.PatientImage, opt => opt.MapFrom(src => src.DiagnosticSession.User.ProfileImage))
                .ForMember(dest => dest.PatientEmail, opt => opt.MapFrom(src => src.DiagnosticSession.User.Email))
                .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.DiagnosticSession.User.PhoneNumber))
                .ForMember(dest => dest.PatientGender, opt => opt.MapFrom(src => src.DiagnosticSession.User.Gender))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DiagnosticSession.DateOfBirth))
                .ForMember(dest => dest.PatientAddress, opt => opt.MapFrom(src => src.DetailedAddress))
               .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
.ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
.ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))

                // Doctor Info (Traverses DiagnosticSession -> Doctor -> User)
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.DiagnosticSession.Doctor.User.Name))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.DiagnosticSession.Doctor.Specialty != null
    ? src.DiagnosticSession.Doctor.Specialty.GetName()
    : null))
                  .ForMember(dest => dest.DoctorContact, opt => opt.MapFrom(src => src.DiagnosticSession.Doctor.User.PhoneNumber))

               // Test Info
               .ForMember(dest => dest.TestName, opt => opt.MapFrom(src =>
    src.MedicalTest != null
    ? src.MedicalTest.GetName()
    : null))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
               .ForMember(dest => dest.ResultFileUrl, opt => opt.MapFrom(src => src.ResultFileUrl)); 

        CreateMap<DiagnosticSessionTest, DoctorOrderDetailDto>()
            // Patient Info
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.DiagnosticSession.User.Name))
            .ForMember(dest => dest.PatientEmail, opt => opt.MapFrom(src => src.DiagnosticSession.User.Email))
             .ForMember(dest => dest.PatientImage, opt => opt.MapFrom(src => src.DiagnosticSession.User.ProfileImage))
            
            .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.DiagnosticSession.User.PhoneNumber))
            .ForMember(dest => dest.MedicalHistorySummary, opt => opt.MapFrom(src => src.DiagnosticSession.MedicalHistory))
             .ForMember(dest => dest.PatientGender, opt => opt.MapFrom(src => src.DiagnosticSession.User.Gender))

           // Test Info
           .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
    src.MedicalTest != null && src.MedicalTest.TestCategory != null
    ? src.MedicalTest.TestCategory.GetName()
    : null))

.ForMember(dest => dest.TestType, opt => opt.MapFrom(src =>
    src.MedicalTest != null
    ? src.MedicalTest.GetName()
    : null))

.ForMember(dest => dest.SampleType, opt => opt.MapFrom(src =>
    src.SampleType != null
    ? src.SampleType.GetName()
    : null))

            // Lab Info
            .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Lab.User.Name))
            .ForMember(dest => dest.ResultFileUrl, opt => opt.MapFrom(src => src.ResultFileUrl));
        CreateMap<DiagnosticSessionTest, DiagnosticSessionMessageTestDto>()
    .ForMember(dest => dest.DiagnosticSessionTestId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.MedicalTest != null && src.MedicalTest.TestCategory != null ? src.MedicalTest.TestCategory.GetName() : null))
    .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.MedicalTest != null ? src.MedicalTest.GetName() : null))
    .ForMember(dest => dest.SampleTypeName, opt => opt.MapFrom(src => src.SampleType != null ? src.SampleType.GetName() : null));
        CreateMap<LabBranch, LabBranchDto>();

       




        CreateMap<DiagnosticSessionTest, DiagnosticSessionMessageTestResultDto>()
    .ForMember(dest => dest.DiagnosticSessionTestId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.MedicalTest != null ? src.MedicalTest.GetName() : null));
        CreateMap<DiagnosticSessionTest, DiagnosticSessionFullTestDto>()
    .ForMember(dest => dest.DiagnosticSessionTestId, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
        src.MedicalTest != null && src.MedicalTest.TestCategory != null ? src.MedicalTest.TestCategory.GetName() : null))
    .ForMember(dest => dest.TestName, opt => opt.MapFrom(src =>
        src.MedicalTest != null ? src.MedicalTest.GetName() : null))
    .ForMember(dest => dest.SampleTypeName, opt => opt.MapFrom(src =>
        src.SampleType != null ? src.SampleType.GetName() : null))
    .ForMember(dest => dest.Result, opt => opt.MapFrom(src =>
        !string.IsNullOrEmpty(src.ResultFileUrl) ? src : null));
        CreateMap<DiagnosticSessionTreatmentPlan, DiagnosticSessionMessageTreatmentPlanDto>()
    
     .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.DiagnosticSessionMessageId))
     .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src =>
         src.Medication != null
         ? (System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar")
             ? src.Medication.NameAr
             : src.Medication.NameEn)
         : null))
     .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
         src.Medication != null ? src.Medication.Price : 0));
        CreateMap<DiagnosticSession, DiagnosticSessionFullDetailsDto>()
      .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests))
      .ForMember(dest => dest.TreatmentPlans, opt => opt.MapFrom(src =>
          (src.TreatmentPlans ?? new List<DiagnosticSessionTreatmentPlan>())
          .Concat(
              (src.Messages ?? new List<DiagnosticSessionMessage>())
              .Where(m => m.DiagnosticSessionTreatmentPlans != null)
              .SelectMany(m => m.DiagnosticSessionTreatmentPlans)
          )
          
          .GroupBy(tp => tp.Id)
          .Select(g => g.First())
          .ToList()
      ));
        CreateMap<Blog, BlogListDto>()
     .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
         src.Author != null && src.Author.User != null
         ? src.Author.User.Name
         : "AlexCare User"))
        .ForMember(dest => dest.AuthorImage, opt => opt.MapFrom(src => src.Author.User.ProfileImage));
        CreateMap<CreateUpdateBlogDto, Blog>()
                .IgnoreFullAuditedObjectProperties()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) 
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore());
        CreateMap<Blog, BlogDetailDto>()
     .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
         src.Author != null && src.Author.User != null
         ? src.Author.User.Name
         : "AlexCare User"))
       .ForMember(dest => dest.AuthorImage, opt => opt.MapFrom(src => src.Author.User.ProfileImage));
        CreateMap<DoctorSchedule, DoctorScheduleDto>().ReverseMap();
        CreateMap<DiagnosticSessionRoom, DiagnosticSessionRoomDto>();
        CreateMap<DiagnosticSessionRoom, VideoSessionSlotDto>()
            .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.DiagnosticSessionId))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.DiagnosticSession.User.Name))
            .ForMember(dest => dest.PatientImageUrl, opt => opt.MapFrom(src => src.
            
            DiagnosticSession.User.ProfileImage))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => (int)(src.ScheduledEndTime - src.ScheduledStartTime).TotalMinutes))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.Status == VideoRoomStatus.Ended))
            .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.VideoRoomId))
          .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
            .ForMember(dest => dest.CanStart, opt => opt.MapFrom(src =>
                src.Status != VideoRoomStatus.Ended &&
                src.Status != VideoRoomStatus.Cancelled &&
                DateTime.Now >= src.ScheduledStartTime.AddMinutes(-10) &&
                DateTime.Now <= src.ScheduledEndTime));
        CreateMap<DoctorRating, DoctorRatingDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.User.Name))
        .ForMember(dest => dest.PatientPictureUrl, opt => opt.MapFrom(src => src.User.ProfileImage))
  
    .AfterMap<DoctorRatingImageUrlMappingAction>();

        // Map from DTO to Entity (for saving new ratings)
        CreateMap<CreateDoctorRatingDto, DoctorRating>()
            .ForMember(dest => dest.DoctorId, opt => opt.Ignore()) // Will be set manually from Session
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        CreateMap<Notification, NotificationDto>();
        CreateMap<RegisterDeviceDto, UserDevice>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}

