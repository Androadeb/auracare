using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Extensions
{
    public class FilterPagedRequest : PagedAndSortedResultRequestDto
    {
        public string SearchTerm { get; set; }
    }
}


