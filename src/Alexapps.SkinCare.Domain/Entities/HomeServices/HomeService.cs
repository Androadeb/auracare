using Alexapps.SkinCare.Entities.Base;
using System;

namespace Alexapps.SkinCare.Entities.HomeServices
{
    public class HomeService : LocalizableEntity
    {
        public decimal Price { get; set; }
        public string Duration { get; set; } // e.g., "60-90 minutes"
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string ServicesAr { get; set; }
        public string ServicesEn { get; set; }
        public string NotesAr { get; set; }
        public string NotesEn { get; set; }
        public string Image { get; set; }
    }
}
