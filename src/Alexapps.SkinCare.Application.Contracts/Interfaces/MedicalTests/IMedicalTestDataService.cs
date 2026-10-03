using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.MedicalTests.Queries;
using Alexapps.SkinCare.Dtos.MedicalTests.Results;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Interfaces.MedicalTests
{
    public interface IMedicalTestDataService : IApplicationService
    {
        Task<List<TestCategoryDto>> GetCategoriesAsync();
        Task<PagedResultWithMetadata<MedicalTestDto>> GetListAsync(GetMedicalTestsInput input);
        Task<List<MedicalTestDto>> GetTestsByCategoryAsync(System.Guid categoryId);
        Task<List<SampleTypeDto>> GetSampleTypesAsync(System.Guid? medicalTestId = null);
    }
}
