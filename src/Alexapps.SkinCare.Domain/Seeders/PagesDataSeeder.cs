using Alexapps.SkinCare.Entities;
using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Seeders;

public class PagesDataSeeder(IRepository<Page, Guid> pageRepo) : ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        if (await pageRepo.AnyAsync())
        {
            return;
        }

        var pages = new List<Page>
        {
            new Page(PageTypeEnum.PrivacyPolicy, "سياسة الخصوصية", "Privacy Policy", "صفحة سياسة الخصوصية", "Privacy Policy Page"),
            new Page(PageTypeEnum.AboutUs, "من نحن", "About Us", "صفحة من نحن", "About Us Page"),
            new Page(PageTypeEnum.TermsAndConditions, "الشروط والأحكام", "Terms And Conditions", "صفحة الشروط والأحكام", "Terms And Conditions Page")
        };

        await pageRepo.InsertManyAsync(pages, true);
    }
}
