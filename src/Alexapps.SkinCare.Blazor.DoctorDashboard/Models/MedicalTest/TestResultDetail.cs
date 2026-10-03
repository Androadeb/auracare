using System.Text.Json.Serialization;

namespace ynex.Models.MedicalTest
{
    public class TestResultDetail
    {

        [JsonPropertyName("diagnosticSessionTestId")]
        public string? DiagnosticSessionTestId { get; set; }

        [JsonPropertyName("testName")]
        public string? TestName { get; set; }

        [JsonPropertyName("resultFileUrl")]
        public string? ResultFileUrl { get; set; }

        [JsonPropertyName("resultFileName")]
        public string? ResultFileName { get; set; }

        // Helper to get the full link for download/preview
        public string FullResultUrl => string.IsNullOrWhiteSpace(ResultFileUrl)
            ? ""
            : (ResultFileUrl.StartsWith("http") ? ResultFileUrl : $"https://auraskin.runasp.net{ResultFileUrl}");
    }
}
