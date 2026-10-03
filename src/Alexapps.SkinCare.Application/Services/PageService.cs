using Alexapps.SkinCare.Dtos.Pages.Commands;
using Alexapps.SkinCare.Dtos.Pages.Results;
using Alexapps.SkinCare.Entities;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Interfaces;
using Alexapps.SkinCare.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Services
{
    public class PageService(
        IRepository<Page, Guid> pageRepo,
        IStringLocalizer<SkinCareResource> localizer
    ) : SkinCareAppService, IPageService
    {
        public async Task<PageDto> GetAsync(PageTypeEnum type)
        {
            var page = await pageRepo.FirstOrDefaultAsync(x => x.Type == type);

            if (page == null)
            {
                throw new UserFriendlyException(localizer["page_not_found"], "404");
            }

            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase);

            return new PageDto
            {
                Type = page.Type,
                Title = page.GetTitle(isArabic),
                Content = page.GetDescription(isArabic)
            };
        }

        public async Task<PageResult> GetForEditAsync(PageTypeEnum type)
        {
            var page = await pageRepo.FirstOrDefaultAsync(x => x.Type == type);

            if (page == null)
            {
                throw new UserFriendlyException(localizer["page_not_found"]);
            }

            return ObjectMapper.Map<Page, PageResult>(page);
        }

        public async Task<PageResult> UpdateAsync(PageTypeEnum type, PageCommand command)
        {
            var page = await pageRepo.FirstOrDefaultAsync(x => x.Type == type);

            if (page == null)
            {
                throw new UserFriendlyException(localizer["page_not_found"]);
            }

            page.DescriptionAr = command.DescriptionAr;
            page.DescriptionEn = command.DescriptionEn;
            page.TitleAr = command.TitleAr;
            page.TitleEn = command.TitleEn;

            page = await pageRepo.UpdateAsync(page, true);

            return ObjectMapper.Map<Page, PageResult>(page);
        }
    }
}
