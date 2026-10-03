using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Entities.Doctors
{
    public class Doctor : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid? SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        public double Rating { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int NumberOfPatients { get; set; }
        public int NumberOfReviews { get; set; }
        public int YearsOfExperience { get; set; }
        public DoctorTypeEnum Type { get; set; }
        public bool CanPublishBlogs { get; set; } = false;
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<DoctorQualification> Qualifications { get; set; } = new List<DoctorQualification>();
    }
}
