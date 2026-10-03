using System;

namespace Alexapps.SkinCare.Dtos.MedicalTests.Queries
{
    public class GetMedicalTestsInput
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public Guid? CategoryId { get; set; }
        public string Filter { get; set; }
    }
}
