using Alexapps.SkinCare.Entities.Base;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Entities.MedicalTests
{
    public class SampleType : LocalizableEntity
    {
        public ICollection<MedicalTest> MedicalTests { get; set; } = new List<MedicalTest>();
    }
}
