using System;

namespace Alexapps.SkinCare.Routes;

public static class ApiRoutes
{
    private const string BaseUrl = "api/v1/";

    public static class Auth
    {
        private const string ModuleUrl = BaseUrl + "auth/";
        public const string Login = ModuleUrl + "login";
        public const string RefreshToken = ModuleUrl + "refresh-token";
        public const string ForgetPassword = ModuleUrl + "forget-password";
        public const string ValidateForgetPasswordOtp = ModuleUrl + "verify-forget-password-otp";
        public const string ResendForgetPasswordOtp = ModuleUrl + "resend-forget-password-otp";
        public const string ChangePassword = ModuleUrl + "change-password";
        public const string VerifyOtp = ModuleUrl + "verify-otp";
        public const string SendOtp = ModuleUrl + "send-otp";
        public const string MyInfo = ModuleUrl + "my-info";
        public const string Logout = ModuleUrl + "logout";
        public const string SignInWithEmailAndSendOtp = ModuleUrl + "sign-in-otp";
        public const string VerifyUpdateOtp = ModuleUrl + "verify-update-otp";
    }

    public static class Client
    {
        private const string ModuleUrl = BaseUrl + "client/";
        public const string Register = ModuleUrl + "register";
    }

    public static class Admin
    {
        private const string ModuleUrl = BaseUrl + "admin/";
        public static class Pages
        {
            private const string PageModuleUrl = ModuleUrl + "pages/";
            public const string Base = PageModuleUrl;
        }
        public static class Medications
        {
            private const string MedicationModuleUrl = ModuleUrl + "medications/";
            public const string Base = MedicationModuleUrl;
            public const string Single = MedicationModuleUrl + "{id}";
        }
    }

    public static class Pages
    {
        private const string ModuleUrl = BaseUrl + "pages/";
        public const string Get = ModuleUrl;
    }

    public static class Doctors
    {
        private const string ModuleUrl = BaseUrl + "doctors/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
        public const string Specialties = ModuleUrl + "specialties";
        public const string Schedules = ModuleUrl + "{id}/schedules";
    }

    public static class HomeServices
    {
        private const string ModuleUrl = BaseUrl + "home-services/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
        public const string ServiceProviders = ModuleUrl + "{homeServiceId}/providers";
    }

    public static class HomeServiceProviders
    {
        private const string ModuleUrl = BaseUrl + "home-service-providers/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
    }

    public static class HomeServiceSessions
    {
        private const string ModuleUrl = BaseUrl + "home-service-sessions/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
        public const string Schedules = ModuleUrl + "schedules";
    }
    //refrence to for CRUD routes to minimize code duplication
    //please read 
    public static class Sample
    {
        private const string ModuleUrl = BaseUrl + "samples/";
        public const string Base = ModuleUrl; //For Get All and Create
        public const string Single = ModuleUrl + "{id}"; //For Get Single, Update and Delete
    }

    public static class MedicalTests
    {
        private const string ModuleUrl = BaseUrl + "medical-tests/";
        public const string Base = ModuleUrl;
        public const string Categories = ModuleUrl + "categories";
        public const string TestsByCategory = ModuleUrl + "categories/{categoryId}/tests";
        public const string SampleTypes = ModuleUrl + "sample-types";
    }

    public static class Medications
    {
        private const string ModuleUrl = BaseUrl + "medications/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
    }

    public static class SkinConditions
    {
        private const string ModuleUrl = BaseUrl + "skin-conditions/";
        public const string Base = ModuleUrl;
    }
    public static class DiagnosticSessions
    {
        private const string ModuleUrl = BaseUrl + "diagnostic-sessions/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
        public const string Messages = ModuleUrl + "messages";
        public const string SessionMessages = ModuleUrl + "{id}/messages";
        public const string FullDetails = ModuleUrl + "{id}/full-details";
        public const string BookTreatmentPlan = ModuleUrl + "book-treatment-plan";
       
    }

    public static class DoctorDiagnosticSessions
    {
        private const string ModuleUrl = BaseUrl + "doctor/diagnostic-sessions/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
        public const string Messages = ModuleUrl + "messages";
        public const string Tests = ModuleUrl + "tests";
        public const string SessionMessages = ModuleUrl + "{id}/messages";
        public const string AddTreatmentPlanItem = ModuleUrl + "add-treatment-item";
        public const string DeleteTreatmentPlanItem = ModuleUrl + "delete-treatment-item/{id}";
        public const string GetAddedTreatmentPlanItems = ModuleUrl + "added-items/{sessionId}";
        public const string SendTreatmentPlan = ModuleUrl + "{id}/send-treatment-plan";
        public const string SendTestResult = ModuleUrl + "send-test-result";

    }
    public static class LabMedicalTests
    {
        
        private const string ModuleUrl = BaseUrl + "lab/medical-tests/";
        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
    }
 
        public static class LabSchedules
        {
            private const string ModuleUrl = BaseUrl + "lab/schedules/";
            public const string Base = ModuleUrl;
            public const string Single = ModuleUrl + "{id}";
            public const string AvailableSlots = ModuleUrl + "available-slots";
        }
    public static class ClientDiagnosticSessionTests
    {
        private const string ModuleUrl = BaseUrl + "client/diagnostic-session-tests/";

        public const string Base = ModuleUrl;
        public const string NearbyLabs = ModuleUrl + "nearby-labs";
        public const string AvailableSlots = ModuleUrl + "available-slots";

      
        public static string ConfirmBooking(Guid id) => $"{ModuleUrl}{id}/confirm-booking";
    }
    public static class LabDiagnosticSessionTests
    {
        private const string ModuleUrl = BaseUrl + "lab/diagnostic-session-tests/";
        public const string Base = ModuleUrl;
        public const string Orders = ModuleUrl + "orders";
        public const string Details = ModuleUrl + "{id}/details";
        public const string UploadResult = ModuleUrl + "{id}/upload-result";
    }
    public static class LabBranches
    {
        private const string ModuleUrl = BaseUrl + "lab/branches/";
        public const string Base = ModuleUrl; 
        public const string Single = ModuleUrl + "{id}";
    }
    public static class DoctorDiagnosticSessionTests
    {
        private const string ModuleUrl = BaseUrl + "doctor/diagnostic-session-tests/";
        public const string Base = ModuleUrl;
        public const string Orders = ModuleUrl + "orders";
        public const string Details = ModuleUrl + "{id}/details";
    }
    public static class DoctorQualifications
    {
        private const string ModuleUrl = BaseUrl + "doctor/qualifications/";

        public const string Base = ModuleUrl; 
        public const string Single = ModuleUrl + "{id}"; 
        public const string ByDoctor = ModuleUrl + "by-doctor/{doctorId}"; 
    }
    public static class Blogs
    {
       
        private const string ModuleUrl = BaseUrl + "doctor/blogs/";

        public const string Base = ModuleUrl;
        public const string Single = ModuleUrl + "{id}";
        public const string ChangeStatus = ModuleUrl + "{id}/status";
    }
    public static class DoctorRatings
    {
        private const string ModuleUrl = BaseUrl + "doctor-ratings/";
        public const string Base = ModuleUrl;
        public const string ByDoctor = ModuleUrl + "doctor/{doctorId}";
        public const string Complete = ModuleUrl + "{id}/complete";
    }
    public static class Notifications
    {
        private const string ModuleUrl = BaseUrl + "notifications/";

        public const string Base = ModuleUrl;
        public const string RegisterDevice = ModuleUrl + "register-device"; 
        public const string Toggle = ModuleUrl + "toggle"; 
        public const string MarkAsRead = ModuleUrl + "{id}/read"; 
    }
}

