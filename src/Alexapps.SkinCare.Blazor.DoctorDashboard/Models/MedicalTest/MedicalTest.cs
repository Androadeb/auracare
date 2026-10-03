using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ynex.Models.MedicalTest
{
    public class MedicalCategory
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("id")] public string Id { get; set; } = "";
    }

    public class SampleTypeItem
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
    }

    public class MedicalTestItem
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
    }

    public class MedicalTestDetail
    {
        [JsonPropertyName("categoryName")] public string CategoryName { get; set; } = "";
        [JsonPropertyName("testName")] public string TestName { get; set; } = "";
        [JsonPropertyName("sampleTypeName")] public string SampleTypeName { get; set; } = "";
        [JsonPropertyName("preparationInstructions")] public string preparationInstructions { get; set; } = "";
    }
    public class CreateMedicalTestRequest
    {
        [Required(ErrorMessage = "MedicalTestRequired")] 
        public string MedicalTestId { get; set; } = string.Empty;

        [Required(ErrorMessage = "SampleTypeRequired")]
        public string SampleTypeId { get; set; } = string.Empty;

        public string PreparationInstructions { get; set; } = string.Empty;

        public string DiagnosticSessionId { get; set; } = string.Empty;
    }
}
