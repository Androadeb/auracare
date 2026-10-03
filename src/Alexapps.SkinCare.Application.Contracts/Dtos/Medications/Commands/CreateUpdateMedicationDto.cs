using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Alexapps.SkinCare.Dtos.Medications.Commands
{
    public class CreateUpdateMedicationDto
    {
        [Required]
        public string NameEn { get; set; }
        [Required]
        public string NameAr { get; set; }
        public string ActiveIngredients { get; set; }
        public string Dosage { get; set; }
        public decimal Price { get; set; }
        public string Uses { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
    }
}
