using Alexapps.SkinCare.Enums;

namespace Alexapps.SkinCare.Dtos.Pages.Results;

public class PageDto
{
    public PageTypeEnum Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
