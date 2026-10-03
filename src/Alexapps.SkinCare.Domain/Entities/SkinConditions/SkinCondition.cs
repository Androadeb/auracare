using Alexapps.SkinCare.Entities.Base;
using System.Globalization;

namespace Alexapps.SkinCare.Entities.SkinConditions
{
    public class SkinCondition : BaseEntity
    {
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Image { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }

        public string GetName()
        {
            return CultureInfo.CurrentUICulture.Name.StartsWith("ar") ? NameAr : NameEn;
        }

        public string GetDescription()
        {
            return CultureInfo.CurrentUICulture.Name.StartsWith("ar") ? DescriptionAr : DescriptionEn;
        }
    }
}
