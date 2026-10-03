using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ynex.Models.Treatment
{
    public class MedicationResponse
    {
        [JsonPropertyName("totalCount")] public int TotalCount { get; set; }
        [JsonPropertyName("items")] public List<MedicationItem> Items { get; set; } = new();
    }

    public class MedicationItem
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("dosage")] public string Dosage { get; set; } = "";
    }

    public class TreatmentPlanDetail
    {
     
        [JsonPropertyName("id")]
        public string? Id { get; set; }

    
        [JsonPropertyName("medicationId")]
        public string? MedicationId { get; set; }

        [Required(ErrorMessage = "MedicationNameRequired")]
        [JsonPropertyName("medicationName")]
        public string? MedicationName { get; set; }

        [Required(ErrorMessage = "DosageRequired")]
        [JsonPropertyName("dosage")]
        public string? Dosage { get; set; }

        [Required(ErrorMessage = "FrequencyRequired")]
        [JsonPropertyName("frequency")]
        public string? Frequency { get; set; } = "once Daily"; 

        [Required(ErrorMessage = "DurationRequired")]
        [JsonPropertyName("duration")]
        public string? Duration { get; set; }

        [Required(ErrorMessage = "QuantityRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "QuantityInvalid")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "RefillsRequired")]
        [JsonPropertyName("refills")]
        public int Refills { get; set; }

        [JsonPropertyName("specialInstructions")]
        public string? SpecialInstructions { get; set; }
    }
}