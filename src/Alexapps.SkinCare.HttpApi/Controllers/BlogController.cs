using Alexapps.SkinCare.Doctors;
using Alexapps.SkinCare.Dtos.Blogs.Queries;
using Alexapps.SkinCare.Dtos.Blogs.Results;
using Alexapps.SkinCare.Dtos.Blogs.Commands;
using Alexapps.SkinCare.Enums;
using Alexapps.SkinCare.Routes; // تم إضافة الـ Namespace الخاص بالـ Routes
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Alexapps.SkinCare.Controllers.Doctors
{
    [RemoteService(Name = "Blog")]
    [Area("app")]
    [ControllerName("Blog")]
    [Route(ApiRoutes.Blogs.Base)]
    [ApiExplorerSettings(GroupName = "doctor")]
    public class BlogController : AbpController, IBlogAppService
    {
        private readonly IBlogAppService _blogAppService;

        public BlogController(IBlogAppService blogAppService)
        {
            _blogAppService = blogAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BlogListDto>> GetListAsync(GetBlogListDto input)
        {
            return _blogAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")] 
        public virtual Task<BlogDetailDto> GetAsync(Guid id)
        {
            return _blogAppService.GetAsync(id);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public virtual Task<BlogListDto> CreateAsync([FromForm] CreateUpdateBlogDto input)
        {
            return _blogAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        [Consumes("multipart/form-data")]
        public virtual Task<BlogListDto> UpdateAsync(Guid id, [FromForm] CreateUpdateBlogDto input)
        {
            return _blogAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _blogAppService.DeleteAsync(id);
        }

        [HttpPatch]
     
        [Route("{id}/status")]
        public virtual Task ChangeStatusAsync(Guid id, BlogStatus status)
        {
            return _blogAppService.ChangeStatusAsync(id, status);
        }
    }
}