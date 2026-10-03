using Alexapps.SkinCare.Dtos.Common;
using Alexapps.SkinCare.Dtos.LabBranches.Commands;
using Alexapps.SkinCare.Dtos.LabBranches.Queries;
using Alexapps.SkinCare.Dtos.LabBranches.Result;
using Alexapps.SkinCare.Entities.LABs;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Alexapps.SkinCare.LabBranches;

public class LabBranchAppService : SkinCareAppService, ILabBranchAppService
{
    private readonly IRepository<LabBranch, Guid> _labBranchRepository;
    private readonly IRepository<Lab, Guid> _labRepository;

    public LabBranchAppService(
        IRepository<LabBranch, Guid> labBranchRepository,
        IRepository<Lab, Guid> labRepository)
    {
        _labBranchRepository = labBranchRepository;
        _labRepository = labRepository;
    }

    public async Task<LabBranchDto> GetAsync(Guid id)
    {
        var branch = await _labBranchRepository.GetAsync(id);
        return ObjectMapper.Map<LabBranch, LabBranchDto>(branch);
    }

    public async Task<PagedResultWithMetadata<LabBranchDto>> GetMyBranchesAsync(GetLabBranchesInput input)
    {
        // Get current lab based on user identity
        var lab = await _labRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
        if (lab == null)
        {
            return new PagedResultWithMetadata<LabBranchDto>(new List<LabBranchDto>(), input.Page, input.Limit, 0);
        }

        // Prepare query for branches belonging to this lab
        var query = (await _labBranchRepository.GetQueryableAsync())
                    .Where(x => x.LabId == lab.Id);

        // Filter by name, contact number, or address
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            var filter = input.Filter.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(filter) ||
                x.ContactNumber.Contains(filter) ||
                x.FullAddress.ToLower().Contains(filter));
        }

        // Count total records before paging
        var totalCount = await AsyncExecuter.CountAsync(query);

        // Apply sorting and paging
        var skipCount = (input.Page - 1) * input.Limit;
        var branches = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.IsPrimary)
                 .ThenBy(x => x.Name)
                 .Skip(skipCount)
                 .Take(input.Limit)
        );

        // Map entities to DTOs
        var dtos = ObjectMapper.Map<List<LabBranch>, List<LabBranchDto>>(branches);

       
        return new PagedResultWithMetadata<LabBranchDto>(dtos, input.Page, input.Limit, totalCount);
    }

    public async Task<LabBranchDto> CreateAsync(CreateLabBranchDto input)
    {
        var lab = await _labRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
        if (lab == null) throw new UserFriendlyException("Lab profile not found.");

        var nameExists = await _labBranchRepository.AnyAsync(x => x.LabId == lab.Id && x.Name.Trim() == input.Name.Trim());
        if (nameExists)
        {
            throw new UserFriendlyException($"A branch with the name '{input.Name}' already exists.");
        }
        var hasExistingBranches = await _labBranchRepository.AnyAsync(x => x.LabId == lab.Id);

        if (!hasExistingBranches)
        {
            input.IsPrimary = true;
        }
        else if (input.IsPrimary)
        {
            var currentPrimary = await _labBranchRepository.FirstOrDefaultAsync(x => x.LabId == lab.Id && x.IsPrimary);
            if (currentPrimary != null)
            {
                currentPrimary.IsPrimary = false;
                await _labBranchRepository.UpdateAsync(currentPrimary);
            }
        }

        var branch = ObjectMapper.Map<CreateLabBranchDto, LabBranch>(input);
        branch.LabId = lab.Id;
        branch.IsActive = true;
        branch.IsPrimary = input.IsPrimary;

        await _labBranchRepository.InsertAsync(branch);
        return ObjectMapper.Map<LabBranch, LabBranchDto>(branch);
    }

    public async Task<LabBranchDto> UpdateAsync(Guid id, UpdateLabBranchDto input)
    {
        var branch = await _labBranchRepository.GetAsync(id);

        // التحقق من تكرار الاسم (مع استثناء الفرع الحالي)
        var nameExists = await _labBranchRepository.AnyAsync(x =>
            x.LabId == branch.LabId &&
            x.Name.Trim() == input.Name.Trim() &&
            x.Id != id);

        if (nameExists)
        {
            throw new UserFriendlyException($"Cannot update: The name '{input.Name}' is already used by another branch.");
        }

      
        if (!input.IsPrimary && branch.IsPrimary)
        {
            var otherBranchesExist = await _labBranchRepository.AnyAsync(x => x.LabId == branch.LabId && x.Id != id);
            if (otherBranchesExist)
            {
                throw new UserFriendlyException("You must assign another branch as primary before changing this branch's status.");
            }
            input.IsPrimary = true;
        }

        if (input.IsPrimary && !branch.IsPrimary)
        {
            var existingPrimary = await _labBranchRepository.FirstOrDefaultAsync(x => x.LabId == branch.LabId && x.IsPrimary);
            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
                await _labBranchRepository.UpdateAsync(existingPrimary);
            }
        }

        ObjectMapper.Map(input, branch);
        await _labBranchRepository.UpdateAsync(branch);
        return ObjectMapper.Map<LabBranch, LabBranchDto>(branch);
    }
    public async Task DeleteAsync(Guid id)
    {
        var branch = await _labBranchRepository.GetAsync(id);

        // Prevent deleting the primary branch if others exist
        if (branch.IsPrimary && await _labBranchRepository.AnyAsync(x => x.LabId == branch.LabId && x.Id != id))
        {
            throw new UserFriendlyException("Please assign another primary branch before deleting this one.");
        }

        await _labBranchRepository.DeleteAsync(id);
    }
}