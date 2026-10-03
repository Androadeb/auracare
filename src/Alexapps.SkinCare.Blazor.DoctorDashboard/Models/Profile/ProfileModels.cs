using System.Text.Json.Serialization;

namespace ynex.Models.Profile
{
    public class ProfileDto
    {
        [JsonPropertyName("name")]
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public Guid? SpecialtyId { get; set; }
        [JsonPropertyName("requiresVerification")]
        public bool RequiresVerification { get; set; }
        public List<QualificationDto> Qualifications { get; set; } = new();
        public BankInfo BankInfo { get; set; } = new();
    }

    public class QualificationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string CertificateImageUrl { get; set; } = string.Empty;
    }
    public class SpecialtyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class BankInfo
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Iban { get; set; } = string.Empty;
        public string RoutingNumber { get; set; } = string.Empty;
    }
}
