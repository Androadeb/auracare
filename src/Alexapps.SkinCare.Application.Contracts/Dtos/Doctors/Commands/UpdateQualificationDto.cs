using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Doctors.Commands
{
    public class UpdateQualificationDto
    {
        public Guid? Id { get; set; } 
        public string Degree { get; set; }
        public IFormFile? CertificateImage { get; set; }
        public string? ExistingImageUrl { get; set; } 
    }
}
