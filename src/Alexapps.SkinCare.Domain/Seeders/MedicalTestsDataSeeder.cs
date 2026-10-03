using Alexapps.SkinCare.Entities.MedicalTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.Seeders
{
    public class MedicalTestsDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<TestCategory, Guid> _categoryRepository;
        private readonly IRepository<MedicalTest, Guid> _testRepository;
        private readonly IRepository<SampleType, Guid> _sampleRepository;

        public MedicalTestsDataSeeder(
            IRepository<TestCategory, Guid> categoryRepository,
            IRepository<MedicalTest, Guid> testRepository,
            IRepository<SampleType, Guid> sampleRepository)
        {
            _categoryRepository = categoryRepository;
            _testRepository = testRepository;
            _sampleRepository = sampleRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Seed Categories
            var categoriesData = new List<(string En, string Ar, List<(string TestEn, string TestAr)> Tests)>
            {
                ("Blood Tests", "تحاليل الدم", new List<(string, string)>
                {
                    ("CBC", "صورة الدم الكاملة"),
                    ("Blood Sugar", "سكر الدم"),
                    ("Liver Function Test", "وظائف الكبد"),
                }),
                ("Urine Tests", "تحاليل البول", new List<(string, string)>()),
                ("Imaging", "الأشعة وتصوير", new List<(string, string)>()),
                ("Hormone Tests", "تحاليل الهرمونات", new List<(string, string)>
                {
                    ("TSH", "هرمون الغدة الدرقية"),
                    ("Vitamin D", "فيتامين د")
                }),
                ("Microbiology", "الأحياء الدقيقة", new List<(string, string)>())
            };

            foreach (var categoryItem in categoriesData)
            {
                var existingCategory = await _categoryRepository.FirstOrDefaultAsync(x => x.NameEn == categoryItem.En);
                if (existingCategory == null)
                {
                    existingCategory = await _categoryRepository.InsertAsync(new TestCategory
                    {
                        NameEn = categoryItem.En,
                        NameAr = categoryItem.Ar
                    }, autoSave: true);
                }

                foreach (var testItem in categoryItem.Tests)
                {
                    if (!await _testRepository.AnyAsync(x => x.NameEn == testItem.TestEn && x.TestCategoryId == existingCategory.Id))
                    {
                        await _testRepository.InsertAsync(new MedicalTest
                        {
                            TestCategoryId = existingCategory.Id,
                            NameEn = testItem.TestEn,
                            NameAr = testItem.TestAr
                        }, autoSave: true);
                    }
                }
            }

            // Seed Sample Types
            var sampleTypesData = new List<(string En, string Ar)>
            {
                ("Blood", "دم"),
                ("Urine", "بول"),
                ("Saliva", "لعاب"),
                ("Stool", "براز"),
                ("Swab", "مسحة")
            };

            foreach (var sampleType in sampleTypesData)
            {
                if (!await _sampleRepository.AnyAsync(x => x.NameEn == sampleType.En))
                {
                    await _sampleRepository.InsertAsync(new SampleType
                    {
                        NameEn = sampleType.En,
                        NameAr = sampleType.Ar
                    }, autoSave: true);
                }
            }

            // Link Sample Types to Medical Tests
            var bloodSample = await _sampleRepository.FirstOrDefaultAsync(x => x.NameEn == "Blood");
            var urineSample = await _sampleRepository.FirstOrDefaultAsync(x => x.NameEn == "Urine");
            var swabSample = await _sampleRepository.FirstOrDefaultAsync(x => x.NameEn == "Swab");

            var allTests = await _testRepository.GetListAsync(includeDetails: true);

            foreach (var test in allTests)
            {
                var category = await _categoryRepository.GetAsync(test.TestCategoryId);
                
                if (test.SampleTypes == null) test.SampleTypes = new List<SampleType>();
                
                if (category.NameEn.Contains("Blood") || category.NameEn.Contains("Hormone"))
                {
                    if (bloodSample != null && !test.SampleTypes.Any(s => s.Id == bloodSample.Id))
                        test.SampleTypes.Add(bloodSample);
                }
                else if (category.NameEn.Contains("Urine"))
                {
                    if (urineSample != null && !test.SampleTypes.Any(s => s.Id == urineSample.Id))
                        test.SampleTypes.Add(urineSample);
                }
                else if (category.NameEn.Contains("Microbiology"))
                {
                    if (swabSample != null && !test.SampleTypes.Any(s => s.Id == swabSample.Id))
                        test.SampleTypes.Add(swabSample);
                    if (bloodSample != null && !test.SampleTypes.Any(s => s.Id == bloodSample.Id))
                        test.SampleTypes.Add(bloodSample);
                }
                else 
                {
                    // Default to blood just so they have something if missed
                    if (bloodSample != null && !test.SampleTypes.Any(s => s.Id == bloodSample.Id))
                        test.SampleTypes.Add(bloodSample);
                }
                
                await _testRepository.UpdateAsync(test, autoSave: true);
            }
        }
    }
}
