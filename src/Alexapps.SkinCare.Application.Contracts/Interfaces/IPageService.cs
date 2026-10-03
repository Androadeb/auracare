using Alexapps.SkinCare.Dtos.Pages.Commands;
using Alexapps.SkinCare.Dtos.Pages.Results;
using Alexapps.SkinCare.Enums;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces
{
    public interface IPageService : IApplicationService
    {
        // Public/Localized
        Task<PageDto> GetAsync(PageTypeEnum type);

        // Admin/Raw
        Task<PageResult> GetForEditAsync(PageTypeEnum type);
        Task<PageResult> UpdateAsync(PageTypeEnum type, PageCommand command);
    }
}
