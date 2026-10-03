using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Dtos.Doctors.Results
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public double Rating { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int NumberOfPatients { get; set; }
        public int NumberOfReviews { get; set; }
        public int YearsOfExperience { get; set; }
        public List<DoctorQualificationDto> Qualifications { get; set; }
        public string Gender { get; set; }
    }
}
