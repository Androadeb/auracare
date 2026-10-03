using Alexapps.SkinCare.Dtos.Blogs.Commands;
using Alexapps.SkinCare.Dtos.Blogs.Queries;
using Alexapps.SkinCare.Dtos.Blogs.Results;

using Alexapps.SkinCare.Enums;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Alexapps.SkinCare.Doctors
{
    public interface IBlogAppService : IApplicationService
    {
     
        Task<PagedResultDto<BlogListDto>> GetListAsync(GetBlogListDto input);

  
        Task<BlogDetailDto> GetAsync(Guid id);

        
        Task<BlogListDto> CreateAsync(CreateUpdateBlogDto input);

   
        Task<BlogListDto> UpdateAsync(Guid id, CreateUpdateBlogDto input);

     
        Task ChangeStatusAsync(Guid id, BlogStatus status);

        Task DeleteAsync(Guid id);
    }
}