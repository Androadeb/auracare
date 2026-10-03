using System;
using Alexapps.SkinCare.Enums;

namespace Alexapps.SkinCare.Dtos.HomeServiceProviders.Queries
{
    public class GetHomeServiceProvidersInput
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string Name { get; set; }
        public UserGenderEnum? Gender { get; set; }
        public Guid? HomeServiceId { get; set; }
    }
}
