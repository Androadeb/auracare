using Alexapps.SkinCare.Entities.Base;
using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Entities.MedicalTests
{
    public class MedicalTest : LocalizableEntity
    {
        public Guid TestCategoryId { get; set; }
        public TestCategory TestCategory { get; set; }
        public ICollection<SampleType> SampleTypes { get; set; } = new List<SampleType>();
    }
}
