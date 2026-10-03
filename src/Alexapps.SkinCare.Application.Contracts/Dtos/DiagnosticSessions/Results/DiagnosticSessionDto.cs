using Alexapps.SkinCare.Dtos.Doctors.Results;
using Alexapps.SkinCare.Dtos.Doctors.Queries;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Results
{
    public class DiagnosticSessionDto : EntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid? DoctorId { get; set; }
        public DoctorDto Doctor { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string ProductsUsed { get; set; }
        public string MedicalHistory { get; set; }
        public string Allergies { get; set; }
        
        public List<string> Images { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string ClientName { get; set; }
        public string ClientImage { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string ClientGender { get; set; }

        public DateTime CreationTime { get; set; }

        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadMessageCount { get; set; }
    }
}
