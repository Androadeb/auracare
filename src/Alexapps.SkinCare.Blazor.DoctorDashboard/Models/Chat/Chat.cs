using System.Text.Json.Serialization;
using ynex.Models.MedicalTest;
using ynex.Models.Treatment;
using static ynex.Pages.Admin.Patient_Info.Patient_Info;

namespace ynex.Models.Chat
{
    public class ChatMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        public string? ClientGuid { get; set; }

        [JsonPropertyName("senderId")]
        public string? SenderId { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("fileUrl")]
        public string? FileUrl { get; set; }
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("roomId")]
        public string? RoomId { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("medicalTest")]
        public MedicalTestDetail? MedicalTest { get; set; }


        [JsonPropertyName("treatmentPlans")] 
        public List<TreatmentPlanDetail> TreatmentPlanItems { get; set; } = new();


        [JsonPropertyName("testResult")]
        public TestResultDetail? TestResult { get; set; }

        [JsonPropertyName("isMe")]
        public bool IsMe { get; set; }

        [JsonPropertyName("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonPropertyName("diagnosticSessionId")]
        public string? DiagnosticSessionId { get; set; }
    }
    public class DoctorScheduleDto
    {
        public Guid Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; } 
        public TimeSpan StartTime { get; set; } 
        public TimeSpan EndTime { get; set; }    
        public bool IsAvailable { get; set; }
    }
    public class ChatResponseWrapper
    {
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("items")]
        public List<ChatMessage> Items { get; set; } = new();
    }
}