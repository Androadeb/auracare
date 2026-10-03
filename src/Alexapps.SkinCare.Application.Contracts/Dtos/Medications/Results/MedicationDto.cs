using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.Medications.Results
{
    public class MedicationDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string ActiveIngredients { get; set; }
        public string Dosage { get; set; }
        public decimal Price { get; set; }
        public List<string> Uses { get; set; }
        public string Description { get; set; }
    }
}
