using Alexapps.SkinCare.Dtos.Blogs.Queries;
using Alexapps.SkinCare.Dtos.Blogs.Results;
using Alexapps.SkinCare.Dtos.Blogs.Commands;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Alexapps.SkinCare.Doctors
{
    [Authorize]
    public class BlogAppService : SkinCareAppService, IBlogAppService
    {
        private readonly IRepository<Blog, Guid> _blogRepository;
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogAppService(
            IRepository<Blog, Guid> blogRepository,
            IRepository<Doctor, Guid> doctorRepository, IWebHostEnvironment webHostEnvironment)
        {
            _blogRepository = blogRepository;
            _doctorRepository = doctorRepository;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<PagedResultDto<BlogListDto>> GetListAsync(GetBlogListDto input)
        {
            var queryable = await _blogRepository.GetQueryableAsync();
            queryable = queryable.Include(x => x.Author).ThenInclude(x => x.User);

            var currentUserId = CurrentUser.GetId();

           
            if (CurrentUser.IsInRole("ADMIN"))
            {
              
            }
        
            else
            {
                var currentDoctor = await _doctorRepository.FirstOrDefaultAsync(x => x.UserId == currentUserId);

                if (currentDoctor != null)
                {
                    
                    queryable = queryable.Where(x =>
                        x.AuthorId == currentDoctor.Id ||
                        x.Status == BlogStatus.Published
                    );
                }
                else
                {
                 
                    queryable = queryable.Where(x => x.Status == BlogStatus.Published);
                }
            }

           
            if (input.Status.HasValue)
            {
                queryable = queryable.Where(x => x.Status == input.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                queryable = queryable.Where(x => x.Title.Contains(input.Filter) ||
                                                 x.Subtitle.Contains(input.Filter));
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var blogs = await AsyncExecuter.ToListAsync(
                queryable.OrderBy(input.Sorting ?? "CreationTime desc")
                         .Skip((input.Page - 1) * input.Limit)
                         .Take(input.Limit)
            );

            return new PagedResultDto<BlogListDto>(
                totalCount,
                ObjectMapper.Map<List<Blog>, List<BlogListDto>>(blogs)
            );
        }

        public async Task<BlogDetailDto> GetAsync(Guid id)
        {
            var blog = await _blogRepository.WithDetailsAsync(x => x.Author, x => x.Author.User)
                .ContinueWith(t => t.Result.FirstOrDefault(x => x.Id == id));

            if (blog == null) throw new EntityNotFoundException(typeof(Blog), id);

            if (!CurrentUser.IsInRole("ADMIN") && blog.Status != BlogStatus.Published)
            {
                var currentDoctor = await _doctorRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.GetId());
                if (currentDoctor == null || blog.AuthorId != currentDoctor.Id)
                {
                    throw new UserFriendlyException("Unauthorized: You cannot view this post yet.");
                }
            }

            return ObjectMapper.Map<Blog, BlogDetailDto>(blog);
        }

        public async Task<BlogListDto> CreateAsync(CreateUpdateBlogDto input)
        {
            var blog = ObjectMapper.Map<CreateUpdateBlogDto, Blog>(input);

        
            if (input.ImageFile != null)
            {
                blog.ImageUrl = await SaveImageAsync(input.ImageFile);
            }

            var currentUserId = CurrentUser.GetId();
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.UserId == currentUserId);

            if (doctor != null)
            {
                if (!doctor.CanPublishBlogs)
                {
                    throw new UserFriendlyException("Access Denied: As a doctor, you need 'CanPublishBlogs' permission to post.");
                }
                blog.AuthorId = doctor.Id;
            }
            else
            {
                blog.AuthorId = null;
            }

            blog.Status = BlogStatus.Pending;

            await _blogRepository.InsertAsync(blog, autoSave: true);
            return ObjectMapper.Map<Blog, BlogListDto>(blog);
        }

        public async Task<BlogListDto> UpdateAsync(Guid id, CreateUpdateBlogDto input)
        {
            var blog = await _blogRepository.GetAsync(id);

            // 1. التحقق من الصلاحيات
            if (!CurrentUser.IsInRole("ADMIN"))
            {
                var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.GetId());
                if (doctor == null || blog.AuthorId != doctor.Id)
                {
                    throw new UserFriendlyException("Unauthorized: You can only edit your own blog posts.");
                }
            }

            ObjectMapper.Map(input, blog);

          
            if (input.ImageFile != null)
            {
                blog.ImageUrl = await SaveImageAsync(input.ImageFile);
            }

            blog.Status = BlogStatus.Pending;

            await _blogRepository.UpdateAsync(blog, autoSave: true);
            return ObjectMapper.Map<Blog, BlogListDto>(blog);
        }
        public async Task ChangeStatusAsync(Guid id, BlogStatus status)
        {
            var blog = await _blogRepository.GetAsync(id);
            blog.Status = status;
            blog.LastStatusChangedBy = CurrentUser.Id;

            await _blogRepository.UpdateAsync(blog);
        }

        public async Task DeleteAsync(Guid id)
        {
            var blog = await _blogRepository.GetAsync(id);
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.UserId == CurrentUser.GetId());

            if (doctor == null || blog.AuthorId != doctor.Id)
            {
                throw new UserFriendlyException("Unauthorized: You can only delete your own blog posts.");
            }

            await _blogRepository.DeleteAsync(id);
        }
        private async Task<string> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
          
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "blogs");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

           
            return "/uploads/blogs/" + fileName;
        }
    }
}