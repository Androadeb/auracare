using Alexapps.SkinCare.Enums;

namespace Alexapps.SkinCare.Dtos.Pages.Results;

public class PageResult
{
    public PageTypeEnum Type { get; set; }
    public string TitleAr { get; set; }
    public string TitleEn { get; set; }
    public string DescriptionAr { get; set; }
    public string DescriptionEn { get; set; }
}
