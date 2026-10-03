using System;

namespace Alexapps.SkinCare.Dtos.SkinConditions.Results
{
    public class SkinConditionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
