using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Command;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Queries;
using Alexapps.SkinCare.Dtos.LabMedicalTest.Result;
using Alexapps.SkinCare.Entities.LABs;
using Alexapps.SkinCare.Entities.MedicalTests;
using Alexapps.SkinCare.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Authorization;

namespace Alexapps.SkinCare.Services
{
    [Authorize]

    public class LabMedicalTestAppService : SkinCareAppService, ILabMedicalTestAppService
    {
        private readonly IRepository<LabMedicalTest, Guid> _labMedicalTestRepository;
        private readonly IRepository<Lab, Guid> _labRepository;
        private readonly IRepository<MedicalTest, Guid> _medicalTestRepository;


        public LabMedicalTestAppService(
            IRepository<LabMedicalTest, Guid> labMedicalTestRepository,
            IRepository<Lab, Guid> labRepository,
            IRepository<MedicalTest, Guid> medicalTestRepository)
        {
            _labMedicalTestRepository = labMedicalTestRepository;
            _labRepository = labRepository;
            _medicalTestRepository = medicalTestRepository;
        }


        private async Task<Guid> GetCurrentLabIdAsync()
        {
            var userId = CurrentUser.Id.GetValueOrDefault();
            var lab = await _labRepository.FirstOrDefaultAsync(l => l.UserId == userId);
            if (lab == null)
            {
                throw new UserFriendlyException("Lab profile not found for the current user.");
            }
            return lab.Id;
        }

        public async Task<LabMedicalTestDto> CreateAsync(CreateLabMedicalTestDto input)
        {
            var labId = await GetCurrentLabIdAsync();

            // 1. التأكد من أن الاختبار الطبي يتبع فعلاً لهذا القسم (Category)
            var medicalTest = await _medicalTestRepository.FirstOrDefaultAsync(x => x.Id == input.MedicalTestId);
            if (medicalTest == null || medicalTest.TestCategoryId != input.TestCategoryId)
            {
                throw new UserFriendlyException("The selected medical test does not belong to the specified category.");
            }

            // 2. التأكد من عدم التكرار (كودك الحالي)
            var exists = await _labMedicalTestRepository.AnyAsync(x =>
                x.LabId == labId && x.MedicalTestId == input.MedicalTestId);

            if (exists)
            {
                throw new UserFriendlyException("This medical test is already added to your lab.");
            }

            var labTest = ObjectMapper.Map<CreateLabMedicalTestDto, LabMedicalTest>(input);
            labTest.LabId = labId;

            await _labMedicalTestRepository.InsertAsync(labTest, autoSave: true);

            return await GetAsync(labTest.Id);
        }
        public async Task<LabMedicalTestDto> UpdateAsync(Guid id, UpdateLabMedicalTestDto input)
        {
            var labId = await GetCurrentLabIdAsync();

            // 1. جلب السجل مع التأكد من ملكية المعمل له
            var labTest = await _labMedicalTestRepository.FirstOrDefaultAsync(x =>
                x.Id == id && x.LabId == labId);

            if (labTest == null)
            {
                throw new UserFriendlyException("Medical test not found in your lab.");
            }

           
            var medicalTest = await _medicalTestRepository.GetAsync(labTest.MedicalTestId);
            if (medicalTest.TestCategoryId != input.TestCategoryId)
            {
                throw new UserFriendlyException("Selected category does not match the original medical test.");
            }

            
            ObjectMapper.Map(input, labTest);

            await _labMedicalTestRepository.UpdateAsync(labTest, autoSave: true);

            return await GetAsync(labTest.Id);
        }

        public async Task DeleteAsync(DeleteLabMedicalTestDto input)
        {
            var labId = await GetCurrentLabIdAsync();
            var labTest = await _labMedicalTestRepository.FirstOrDefaultAsync(x =>
            x.Id == input.Id && x.LabId == labId);

            if (labTest != null)
            {
                await _labMedicalTestRepository.DeleteAsync(labTest);
            }
        }

        private async Task<LabMedicalTestDto> GetAsync(Guid id)
        {
            var query = await _labMedicalTestRepository.GetQueryableAsync();

            var labTest = await query
                .Include(x => x.MedicalTest).ThenInclude(mt => mt.TestCategory)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (labTest == null)
            {
                throw new UserFriendlyException("Test not found.");
            }

            return ObjectMapper.Map<LabMedicalTest, LabMedicalTestDto>(labTest);
        }

        public async Task<PagedResultWithMetadata<LabMedicalTestDto>> GetListAsync(GetLabMedicalTestListDto input)
        {
            var labId = await GetCurrentLabIdAsync();
            var query = await _labMedicalTestRepository.GetQueryableAsync();

         
            query = query
                .Include(x => x.MedicalTest).ThenInclude(mt => mt.TestCategory)
                .Where(x => x.LabId == labId);

          
            if (input.IsEnabled.HasValue)
            {
                query = query.Where(x => x.IsEnabled == input.IsEnabled.Value);
            }

           
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                var filterLower = input.Filter.ToLower();
                query = query.Where(x =>
                    x.TestCode.ToLower().Contains(filterLower) ||
                    x.MedicalTest.NameAr.Contains(input.Filter) || 
                    x.MedicalTest.NameEn.ToLower().Contains(filterLower));
            }

            var totalCount = await AsyncExecuter.CountAsync(query);

            // 4. الترتيب والـ Paging
            var list = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.CreationTime)
                     .Skip((input.Page - 1) * input.Limit)
                     .Take(input.Limit)
            );

            var dtos = ObjectMapper.Map<List<LabMedicalTest>, List<LabMedicalTestDto>>(list);
            return new PagedResultWithMetadata<LabMedicalTestDto>(dtos, input.Page, input.Limit, totalCount);
        }
    }
}

