using System.Text.Json.Serialization;

namespace ynex.Models.Sessions
{
    public class SessionResponse
    {
        public List<SessionData> Items { get; set; } = new();
        public PaginationMetadata Metadata { get; set; } = new(); 
    }

    public class SessionData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("clientName")]
        public string clientName { get; set; } = ""; // Matches @session.clientName

        [JsonPropertyName("clientImage")]
        public string clientImage { get; set; } = ""; // Matches @session.clientImage

        [JsonPropertyName("clientEmail")]
        public string clientEmail { get; set; } = ""; // Matches @session.clientEmail

        [JsonPropertyName("clientPhone")]
        public string clientPhone { get; set; } = ""; // Matches @session.clientPhone

        [JsonPropertyName("clientGender")]
        public string clientGender { get; set; } = ""; // Matches @session.clientGender

        [JsonPropertyName("status")]
        public string status { get; set; } = ""; // Matches @session.status

        [JsonPropertyName("description")]
        public string description { get; set; } = ""; // Matches @session.description

        [JsonPropertyName("medicalHistory")]
        public string medicalHistory { get; set; } = ""; // Matches @session.medicalHistory

        [JsonPropertyName("allergies")]
        public string allergies { get; set; } = ""; // Matches @session.allergies

        [JsonPropertyName("images")]
        public List<string> images { get; set; } = new(); // Matches @session.images

        [JsonPropertyName("creationTime")]
        public DateTime creationTime { get; set; } // Matches @session.creationTime
    }
    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    public class SessionAuthResponse
    {
        [JsonPropertyName("accessToken")] public string AccessToken { get; set; }
        [JsonPropertyName("refreshToken")] public string RefreshToken { get; set; }
    }
}