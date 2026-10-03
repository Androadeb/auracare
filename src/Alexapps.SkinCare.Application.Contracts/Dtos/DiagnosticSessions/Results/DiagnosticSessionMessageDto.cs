using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Results
{
    public class DiagnosticSessionMessageDto : AuditedEntityDto<Guid>
    {
        public Guid DiagnosticSessionId { get; set; }
        public Guid SenderId { get; set; }
        
        public DiagnosticSessionMessageType Type { get; set; }
        
        // For Text messages
        public string Message { get; set; }
        public DiagnosticSessionMessageTestResultDto TestResult { get; set; }
        // For Test messages
        public DiagnosticSessionMessageTestDto MedicalTest { get; set; }

        // For Treatment Plan messages
        public List<DiagnosticSessionMessageTreatmentPlanDto> TreatmentPlans { get; set; } = new List<DiagnosticSessionMessageTreatmentPlanDto>();

        public string Token { get; set; }
        public string RoomId { get; set; }
        // For File messages
        public string FileUrl { get; set; }
        
        public bool IsRead { get; set; }
        public bool IsMe { get; set; }
        public string Status { get; set; }
    }
    public class DiagnosticSessionMessageTestResultDto
    {
        public Guid DiagnosticSessionTestId { get; set; }
        public string TestName { get; set; }
        public string ResultFileUrl { get; set; }
        public string ResultFileName { get; set; }
    }
    public class DiagnosticSessionMessageTestDto
    {
        public Guid DiagnosticSessionTestId { get; set; }
        public string CategoryName { get; set; }
        public string TestName { get; set; }
        public string SampleTypeName { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool IsPaid { get; set; }
        public string PreparationInstructions { get; set; }
    }

    public class DiagnosticSessionMessageTreatmentPlanDto
    {
        public Guid Id { get; set; }
        public Guid? MessageId { get; set; }
        public string? OrderNumber { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public int? Refills { get; set; }
        public string SpecialInstructions { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? DetailedAddress { get; set; }
        public bool IsPaid { get; set; } = false;
        public string Status { get; set; }
    }
    public class DiagnosticSessionFullTestDto
    {
        public Guid DiagnosticSessionTestId { get; set; }
        public Guid? MessageId { get; set; }
        public string CategoryName { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public string SampleTypeName { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool IsPaid { get; set; }
        public string PreparationInstructions { get; set; }

        public DiagnosticSessionMessageTestResultDto? Result { get; set; }
    }
}
