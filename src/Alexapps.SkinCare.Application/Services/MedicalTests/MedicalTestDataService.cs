using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.MedicalTests.Queries;
using Alexapps.SkinCare.Dtos.MedicalTests.Results;
using Alexapps.SkinCare.Entities.MedicalTests;
using Alexapps.SkinCare.Interfaces.MedicalTests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Alexapps.SkinCare.Services.MedicalTests
{
    public class MedicalTestDataService : SkinCareAppService, IMedicalTestDataService
    {
        private readonly IRepository<TestCategory, Guid> _testCategoryRepository;
        private readonly IRepository<MedicalTest, Guid> _medicalTestRepository;
        private readonly IRepository<SampleType, Guid> _sampleTypeRepository;

        public MedicalTestDataService(
            IRepository<TestCategory, Guid> testCategoryRepository,
            IRepository<MedicalTest, Guid> medicalTestRepository,
            IRepository<SampleType, Guid> sampleTypeRepository)
        {
            _testCategoryRepository = testCategoryRepository;
            _medicalTestRepository = medicalTestRepository;
            _sampleTypeRepository = sampleTypeRepository;
        }

        public async Task<List<TestCategoryDto>> GetCategoriesAsync()
        {
            var categories = await _testCategoryRepository.GetListAsync();
            return ObjectMapper.Map<List<TestCategory>, List<TestCategoryDto>>(categories);
        }

        public async Task<PagedResultWithMetadata<MedicalTestDto>> GetListAsync(GetMedicalTestsInput input)
        {
            var query = await _medicalTestRepository.GetQueryableAsync();
            
            if (input.CategoryId.HasValue)
            {
                query = query.Where(x => x.TestCategoryId == input.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(x => x.NameAr.Contains(input.Filter) || x.NameEn.Contains(input.Filter));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);
            
            var tests = await AsyncExecuter.ToListAsync(query
                .OrderBy(x => x.NameEn)
                .Skip((input.Page - 1) * input.Limit)
                .Take(input.Limit));

            var dtos = ObjectMapper.Map<List<MedicalTest>, List<MedicalTestDto>>(tests);
            return new PagedResultWithMetadata<MedicalTestDto>(dtos, input.Page, input.Limit, totalCount);
        }

        public async Task<List<SampleTypeDto>> GetSampleTypesAsync(Guid? medicalTestId = null)
        {
            var query = await _sampleTypeRepository.GetQueryableAsync();
            
            if (medicalTestId.HasValue)
            {
                query = query.Where(x => x.MedicalTests.Any(m => m.Id == medicalTestId.Value));
            }

            var sampleTypes = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<SampleType>, List<SampleTypeDto>>(sampleTypes);
        }

        public async Task<List<MedicalTestDto>> GetTestsByCategoryAsync(Guid categoryId)
        {
            var query = await _medicalTestRepository.GetQueryableAsync();
            var tests = await AsyncExecuter.ToListAsync(query.Where(x => x.TestCategoryId == categoryId));
            return ObjectMapper.Map<List<MedicalTest>, List<MedicalTestDto>>(tests);
        }
    }
}
