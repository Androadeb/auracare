using Alexapps.SkinCare.Dtos.Medications.Commands;
using Alexapps.SkinCare.Dtos.Medications.Results;
using Alexapps.SkinCare.Entities.Medications;
using Alexapps.SkinCare.Interfaces.Medications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Alexapps.SkinCare.Services.Medications
{
    public class MedicationService : SkinCareAppService, IMedicationService
    {
        private readonly IRepository<Medication, Guid> _medicationRepository;

        public MedicationService(IRepository<Medication, Guid> medicationRepository)
        {
            _medicationRepository = medicationRepository;
        }

        public async Task<MedicationDto> CreateAsync(CreateUpdateMedicationDto input)
        {
            var medication = new Medication
            {
                NameEn = input.NameEn,
                NameAr = input.NameAr,
                ActiveIngredients = input.ActiveIngredients,
                Dosage = input.Dosage,
                Price = input.Price,
                Uses = input.Uses,
                DescriptionEn = input.DescriptionEn,
                DescriptionAr = input.DescriptionAr
            };

            await _medicationRepository.InsertAsync(medication, autoSave: true);
            return MapToDto(medication);
        }

        public async Task<MedicationDto> UpdateAsync(Guid id, CreateUpdateMedicationDto input)
        {
            var medication = await _medicationRepository.GetAsync(id);
            
            medication.NameEn = input.NameEn;
            medication.NameAr = input.NameAr;
            medication.ActiveIngredients = input.ActiveIngredients;
            medication.Dosage = input.Dosage;
            medication.Price = input.Price;
            medication.Uses = input.Uses;
            medication.DescriptionEn = input.DescriptionEn;
            medication.DescriptionAr = input.DescriptionAr;

            await _medicationRepository.UpdateAsync(medication, autoSave: true);
            return MapToDto(medication);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _medicationRepository.DeleteAsync(id);
        }

        public async Task<MedicationDto> GetAsync(Guid id)
        {
            var medication = await _medicationRepository.GetAsync(id);
            return MapToDto(medication);
        }

        public async Task<List<MedicationDto>> GetAllAsync()
        {
            var medications = await _medicationRepository.GetListAsync();
            return medications.Select(MapToDto).ToList();
        }

        public async Task<PagedResultDto<MedicationDto>> GetListAsync(string keyword, PagedAndSortedResultRequestDto input)
        {
            var queryable = await _medicationRepository.GetQueryableAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryable = queryable.Where(x => x.NameEn.Contains(keyword) 
                                             || x.NameAr.Contains(keyword) 
                                             || x.ActiveIngredients.Contains(keyword));
            }
            
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            var medications = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(m => m.NameEn).Skip(input.SkipCount).Take(input.MaxResultCount)
            );

            return new PagedResultDto<MedicationDto>(
                totalCount,
                medications.Select(MapToDto).ToList()
            );
        }

        private MedicationDto MapToDto(Medication m)
        {
            var isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            return new MedicationDto
            {
                Id = m.Id,
                Name = isArabic ? m.NameAr : m.NameEn,
                ActiveIngredients = m.ActiveIngredients,
                Dosage = m.Dosage,
                Price = m.Price,
                Uses = m.Uses?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(),
                Description = isArabic ? m.DescriptionAr : m.DescriptionEn
            };
        }
    }
}
