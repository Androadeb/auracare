using Alexapps.SkinCare.Entities.Base;

namespace Alexapps.SkinCare.Entities.Medications
{
    public class Medication : LocalizableEntity
    {
        public string ActiveIngredients { get; set; }
        public string Dosage { get; set; }
        public decimal Price { get; set; }
        public string Uses { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
    }
}
