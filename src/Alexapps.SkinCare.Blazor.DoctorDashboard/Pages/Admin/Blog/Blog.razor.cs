using Alexapps.SkinCare.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ynex.Models.Auth;
using ynex.Models.Blog;

namespace Alexapps.SkinCare.Blazor.Pages.Admin
{
    public partial class Blog : ComponentBase
    {
        [Inject] public IBlogService BlogService { get; set; } = default!;
        [Inject] private AuthState Auth { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;

        private bool isAddingPost = false;
        private bool isEditing = false;
        private bool isLoading = false;
        private Guid currentEditingId;
        private IBrowserFile? selectedFile;

        // Dictionary to manage the visibility of the dropdown for each post
        private Dictionary<Guid, bool> openDropdowns = new();

        protected List<BlogModel> BlogPosts { get; set; } = new();
        private BlogModel newPost = new();
        private void NavigateToDetails(Guid postId)
        {
            Navigation.NavigateTo($"/admin/blog/details/{postId}");
        }
        protected override async Task OnInitializedAsync()
        {
            await GetBlogPostsAsync();
        }

        // Logic to toggle dropdown visibility on click
        private void ToggleDropdown(Guid postId)
        {
            if (openDropdowns.ContainsKey(postId))
            {
                openDropdowns[postId] = !openDropdowns[postId];
            }
            else
            {
                // Optional: Close all other open dropdowns before opening the new one
                openDropdowns.Clear();
                openDropdowns[postId] = true;
            }
        }

        private async Task GetBlogPostsAsync()
        {
            isLoading = true;
            try
            {
                // تم حذف فحص التوكن وتمريره هنا
                var response = await BlogService.GetBlogsAsync();
                BlogPosts = response?.Items ?? new List<BlogModel>();

                openDropdowns.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching blogs: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task HandleSubmit()
        {
            if (string.IsNullOrWhiteSpace(newPost.Title)) return;
            isLoading = true;

            try
            {
                var richContent = await JSRuntime.InvokeAsync<string>("getQuillContent", "#editor-container");
                newPost.Content = richContent;

                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(newPost.Title ?? ""), "Title");
                content.Add(new StringContent(newPost.Subtitle ?? ""), "Subtitle");
                content.Add(new StringContent(newPost.Content ?? ""), "Content");
                content.Add(new StringContent("0"), "Status");

                if (selectedFile != null)
                {
                    var maxFileSize = 10 * 1024 * 1024; // 10MB
                    var stream = selectedFile.OpenReadStream(maxFileSize);
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);

                    content.Add(fileContent, "ImageFile", selectedFile.Name);
                }

                bool success = false;
                if (isEditing)
                {
                    // تم حذف التوكن من هنا
                    success = await BlogService.UpdateBlogAsync(currentEditingId, content);
                }
                else
                {
                    // تم حذف التوكن من هنا
                    success = await BlogService.CreateBlogAsync(content);
                }

                if (success)
                {
                    HideAddForm();
                    await GetBlogPostsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[C# Upload Error]: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
        private async Task ConfirmDelete(Guid id)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this post?");
            if (confirmed)
            {
              
                var success = await BlogService.DeleteBlogAsync(id);
                if (success)
                {
                    await GetBlogPostsAsync();
                }
            }
        }

        private async Task ShowAddForm()
        {
            newPost = new BlogModel { PublishDate = DateTime.Now };
            isEditing = false;
            isAddingPost = true;
            selectedFile = null;

          
            await Task.Delay(100);
            await JSRuntime.InvokeVoidAsync("initializeQuill", "#editor-container");
        }

        private async Task ShowEditForm(BlogModel post) // أضفنا async Task
        {
            isEditing = true;
            currentEditingId = post.Id;

            newPost = new BlogModel
            {
                Id = post.Id,
                Title = post.Title,
                Subtitle = post.Subtitle,
                Content = post.Content,
                CoverImage = post.FullCoverImage
            };

            isAddingPost = true;
            selectedFile = null;

            StateHasChanged();

            await Task.Delay(100);

         
            await JSRuntime.InvokeVoidAsync("initializeQuill", "#editor-container");
        }

        private void HideAddForm()
        {
            isAddingPost = false;
            isEditing = false;
            newPost = new();
            selectedFile = null;
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;

            if (selectedFile != null)
            {
                try
                {
                  
                    var maxFileSize = 1024 * 1024 * 5;

                    // 2. فتح Stream للملف
                    using var stream = selectedFile.OpenReadStream(maxFileSize);

                    // 3. تحويل الـ Stream إلى مرجع يفهمه JavaScript
                    using var streamRef = new DotNetStreamReference(stream);

                    // 4. استدعاء JS لإعطائنا رابط المعاينة
                    var previewUrl = await JSRuntime.InvokeAsync<string>("createPreviewUrl", streamRef);

                    newPost.CoverImage = previewUrl;
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Preview error: {ex.Message}");
                }
            }
        }

        private string GetStatusStyle(object status)
        {
            string s = status?.ToString() ?? "";
            return s switch
            {
                "0" => "background-color: #fef3c7 !important; color: #d97706 !important;",
                "1" => "background-color: #dcfce7 !important; color: #15803d !important;",
                "2" => "background-color: #fee2e2 !important; color: #ef4444 !important;",
                _ => "background-color: #f3f4f6; color: #6b7280;"
            };
        }

        private string GetStatusLabel(object status)
        {
            string s = status?.ToString() ?? "";
            return s switch
            {
                "0" => "Pending Review",
                "1" => "Published",
                "2" => "Rejected",
                _ => "Draft"
            };
        }
    }
}