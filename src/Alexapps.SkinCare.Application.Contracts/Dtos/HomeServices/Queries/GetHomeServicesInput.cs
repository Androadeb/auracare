using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.HomeServices.Queries
{
    public class GetHomeServicesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
