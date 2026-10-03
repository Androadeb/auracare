using Alexapps.SkinCare.Entities.SkinConditions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Seeders
{
    public class SkinConditionsDataSeeder : ITransientDependency
    {
        private readonly IRepository<SkinCondition, Guid> _skinConditionRepository;
        private readonly IDataFilter _dataFilter;

        public SkinConditionsDataSeeder(
            IRepository<SkinCondition, Guid> skinConditionRepository,
            IDataFilter dataFilter)
        {
            _skinConditionRepository = skinConditionRepository;
            _dataFilter = dataFilter;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _skinConditionRepository.AnyAsync())
            {
                return;
            }
            await SeedSkinConditionsAsync();
        }

        private async Task SeedSkinConditionsAsync()
        {
            // Disable soft-delete filter to get ALL skin conditions (including soft-deleted ones)
            using (_dataFilter.Disable<ISoftDelete>())
            {
                // Hard delete all existing skin conditions first
                var existingConditions = await _skinConditionRepository.GetListAsync();
                foreach (var condition in existingConditions)
                {
                    await _skinConditionRepository.HardDeleteAsync(condition, autoSave: true);
                }
            }

            var skinConditions = new List<(string NameEn, string NameAr, string DescriptionEn, string DescriptionAr, string Image)>
            {
                ("Acne", "حب الشباب", 
                    "A common skin condition that occurs when hair follicles become clogged with oil and dead skin cells.",
                    "حالة جلدية شائعة تحدث عندما تنسد بصيلات الشعر بالزيت وخلايا الجلد الميتة.",
                    "/assets/img/SkinCondition/skin.png"),
                ("Dry Skin", "جفاف الجلد", 
                    "A condition where the skin lacks moisture, causing it to feel tight, rough, and flaky.",
                    "حالة يفتقر فيها الجلد للرطوبة، مما يجعله يشعر بالشد والخشونة والتقشر.",
                    "/assets/img/SkinCondition/dry-skin.png"),
                ("Eczema", "إكزيما", 
                    "A condition that makes skin red, inflamed, itchy, cracked, and rough.",
                    "حالة تجعل الجلد أحمر وملتهباً ومثيراً للحكة ومتشققاً وخشناً.",
                    "/assets/img/SkinCondition/dermatology.png"),
                ("Skin Hydration", "ترطيب البشرة", 
                    "The process of maintaining adequate moisture levels in the skin for a healthy appearance.",
                    "عملية الحفاظ على مستويات رطوبة كافية في الجلد للحصول على مظهر صحي.",
                    "/assets/img/SkinCondition/hydration.png"),
                ("Skin Brightness", "نضارة البشرة", 
                    "Achieving a radiant, glowing complexion through proper skincare and treatments.",
                    "الحصول على بشرة مشرقة ومتوهجة من خلال العناية بالبشرة والعلاجات المناسبة.",
                    "/assets/img/SkinCondition/brightness.png")
            };

            foreach (var conditionData in skinConditions)
            {
                await _skinConditionRepository.InsertAsync(new SkinCondition
                {
                    NameEn = conditionData.NameEn,
                    NameAr = conditionData.NameAr,
                    DescriptionEn = conditionData.DescriptionEn,
                    DescriptionAr = conditionData.DescriptionAr,
                    Image = conditionData.Image
                }, autoSave: true);
            }
        }
    }
}
