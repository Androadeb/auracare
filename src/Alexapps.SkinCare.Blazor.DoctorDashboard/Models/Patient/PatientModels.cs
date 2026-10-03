using System.Text.Json.Serialization;

namespace ynex.Models.Patient
{
    public class AuthResponse
    {
        [JsonPropertyName("accessToken")] public string AccessToken { get; set; } = "";
        [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; } = "";
    }

    public class PatientSessionItem
    {
        [JsonPropertyName("id")] public string id { get; set; } = "";
        [JsonPropertyName("clientName")] public string clientName { get; set; } = "";
        [JsonPropertyName("clientImage")] public string clientImage { get; set; } = "";
        [JsonPropertyName("clientEmail")] public string clientEmail { get; set; } = "";
        [JsonPropertyName("clientPhone")] public string clientPhone { get; set; } = "";
        [JsonPropertyName("clientGender")] public string clientGender { get; set; } = "";
        [JsonPropertyName("dateOfBirth")] public DateTime? dateOfBirth { get; set; }
        [JsonPropertyName("status")] public string status { get; set; } = "";
        [JsonPropertyName("description")] public string description { get; set; } = "";
        [JsonPropertyName("medicalHistory")] public string medicalHistory { get; set; } = "";
        [JsonPropertyName("allergies")] public string allergies { get; set; } = "";
        [JsonPropertyName("images")] public List<string> images { get; set; } = new();
        [JsonPropertyName("creationTime")] public DateTime creationTime { get; set; }
    }
}
