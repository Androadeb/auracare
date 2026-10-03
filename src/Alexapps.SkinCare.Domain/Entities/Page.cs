using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Enums;

namespace Alexapps.SkinCare.Entities
{
    public class Page : BaseEntity
    {
        public Page(PageTypeEnum type, string titleAr, string titleEn, string descriptionAr, string descriptionEn)
        {
            Type = type;
            TitleAr = titleAr;
            TitleEn = titleEn;
            DescriptionAr = descriptionAr;
            DescriptionEn = descriptionEn;
        }
        public PageTypeEnum Type { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }

        public string GetTitle(bool isArabic)
        {
            return isArabic ? TitleAr : TitleEn;
        }
        public string GetDescription(bool isArabic)
        {
            return isArabic ? DescriptionAr : DescriptionEn;
        }
    }
}

