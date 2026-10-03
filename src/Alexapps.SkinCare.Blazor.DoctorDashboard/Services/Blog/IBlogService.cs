using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http; 
using ynex.Models.Blog;

namespace Alexapps.SkinCare.Blazor.Services
{
    public interface IBlogService
    {
        Task<BlogListResponse> GetBlogsAsync();
        Task<bool> CreateBlogAsync(MultipartFormDataContent content);
        Task<BlogModel?> GetByIdAsync(Guid id);
        Task<bool> UpdateBlogAsync(Guid id, MultipartFormDataContent content);
        Task<bool> DeleteBlogAsync(Guid id);
    }
}