using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.MedicalTests.Queries;
using Alexapps.SkinCare.Dtos.MedicalTests.Results;
using Alexapps.SkinCare.Interfaces.MedicalTests;
using Alexapps.SkinCare.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Controllers
{
    [Authorize]
    public class MedicalTestDataController(IMedicalTestDataService medicalTestDataService) : SkinCareController
    {
        [HttpGet(ApiRoutes.MedicalTests.Categories)]
        public async Task<List<TestCategoryDto>> GetCategoriesAsync()
        {
            return await medicalTestDataService.GetCategoriesAsync();
        }

        [HttpGet(ApiRoutes.MedicalTests.Base)]
        public async Task<PagedResultWithMetadata<MedicalTestDto>> GetListAsync([FromQuery] GetMedicalTestsInput input)
        {
            return await medicalTestDataService.GetListAsync(input);
        }

        [HttpGet(ApiRoutes.MedicalTests.TestsByCategory)]
        public async Task<List<MedicalTestDto>> GetTestsByCategoryAsync(Guid categoryId)
        {
            return await medicalTestDataService.GetTestsByCategoryAsync(categoryId);
        }

        [HttpGet(ApiRoutes.MedicalTests.SampleTypes)]
        public async Task<List<SampleTypeDto>> GetSampleTypesAsync([FromQuery] Guid? medicalTestId = null)
        {
            return await medicalTestDataService.GetSampleTypesAsync(medicalTestId);
        }
    }
}
