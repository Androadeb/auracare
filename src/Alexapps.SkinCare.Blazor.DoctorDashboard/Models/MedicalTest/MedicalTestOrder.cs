using System.Text.Json.Serialization;

namespace ynex.Models.MedicalTest
{
    public class MedicalTestOrder
    {
        public string id { get; set; }
        public string patientName { get; set; }

        public string? patientImage { get; set; }
        public string testType { get; set; }
        public DateTime requestedDate { get; set; }
        public string status { get; set; }
      
    }
    public class MedicalTestOrderResponse
    {
        public List<MedicalTestOrder> items { get; set; } = new();
       
    }
    public class MedicalTestOrderDetail
    {
       
        public string id { get; set; } = string.Empty;
      
        public string? diagnosticSessionId { get; set; }
        public string? DiagnosticSessionTestId { get; set; }
        public string patientName { get; set; } = string.Empty;
        public string patientEmail { get; set; } = string.Empty;
        public string patientPhone { get; set; } = string.Empty;
        public string patientGender { get; set; } = string.Empty;
        public string medicalHistorySummary { get; set; } = string.Empty;
        public string age { get; set; } = "28 years";
        public string? patientImage { get; set; }

        // Test Information
        public string testType { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public string sampleType { get; set; } = string.Empty;
        public string labName { get; set; } = string.Empty;

        // Attachment Path
        public string? resultFileName { get; set; }
        public string? resultFileUrl { get; set; }

        // --- Professional Computed Properties ---

        // رابط الملف الكامل للعرض (Preview)
        public string FullFileUrl => BuildUrl(resultFileUrl);

        // رابط صورة المريض الكامل أو صورة افتراضية
        public string FullPatientImageUrl => !string.IsNullOrWhiteSpace(patientImage)
            ? BuildUrl(patientImage)
            : "assets/images/faces/1.jpg";

   
        public string FileName
        {
            get
            {
                
                if (!string.IsNullOrWhiteSpace(resultFileName))
                    return resultFileName;

             
                if (!string.IsNullOrWhiteSpace(resultFileUrl))
                    return resultFileUrl.Split('/').LastOrDefault() ?? "Medical_Report.pdf";

                return "No attachment";
            }
        }

       
        private string BuildUrl(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return path;

            var baseUrl = "https://auraskin.runasp.net";
            return $"{baseUrl.TrimEnd('/')}/{(path.TrimStart('/'))}";
        }
    }
}
