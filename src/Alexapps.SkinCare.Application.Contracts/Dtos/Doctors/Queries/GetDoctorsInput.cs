using System;
using Alexapps.SkinCare.Enums;

namespace Alexapps.SkinCare.Dtos.Doctors.Queries
{
    public class GetDoctorsInput
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Name { get; set; }
        public Guid? SpecialtyId { get; set; }
        public UserGenderEnum? Gender { get; set; }
        public bool SortByTopRated { get; set; } = true;
    }
}
