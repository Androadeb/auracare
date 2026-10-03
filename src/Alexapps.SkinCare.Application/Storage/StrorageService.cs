using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Alexapps.SkinCare.Configurations;

namespace Alexapps.SkinCare.Integrations.Storage;

public class StorageService : IStorageService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly StorageConfiguration _storageConfiguration;
    private readonly IConfiguration _configuration;

    public StorageService(
        IHttpContextAccessor httpContextAccessor, 
        StorageConfiguration storageConfiguration,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _storageConfiguration = storageConfiguration;
        _configuration = configuration;
        EnsureUploadsDirectoryExists();
    }

    private void EnsureUploadsDirectoryExists()
    {
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }
    }
    public async Task<string> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new Volo.Abp.UserFriendlyException("File is empty or null.");
        }

        try 
        {
            var extension = Path.GetExtension(file.FileName);
            var filename = StorageExtensions.GetNewName() + extension;
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var path = Path.Combine(directoryPath, filename);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = "/uploads/" + filename;
            return $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}{url}";
        }
        catch (Exception ex)
        {
            throw new Volo.Abp.UserFriendlyException($"File upload failed: {ex.Message}");
        }
    }

    public async Task<string> Upload(IFormFile file, string location)
    {
        if (file == null || file.Length == 0)
        {
            throw new Volo.Abp.UserFriendlyException("File is empty or null.");
        }

        try
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", location);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var extension = Path.GetExtension(file.FileName);
            var filename = StorageExtensions.GetNewName() + extension;
            var filePath = Path.Combine(directoryPath, filename);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = GetBaseUrl();
            var relativeUrl = $"/uploads/{location}/{filename}";
            return baseUrl + relativeUrl;
        }
        catch (Exception ex)
        {
            throw new Volo.Abp.UserFriendlyException($"File upload failed: {ex.Message}");
        }
    }

    private string GetBaseUrl()
    {
        // First try configuration from App:SelfUrl
        var baseUrl = _configuration["App:SelfUrl"];
        
        // Then try Storage:BaseUrl for backward compatibility
        if (string.IsNullOrEmpty(baseUrl))
        {
            baseUrl = _storageConfiguration?.BaseUrl;
        }
        
        // Then try HttpContext if available
        if (string.IsNullOrEmpty(baseUrl) && _httpContextAccessor?.HttpContext != null)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        }
        
        return baseUrl ?? "";
    }

    public async Task<bool> Delete(string fileUrl)
    {
        try
        {
            var hostUrl = StorageExtensions.GetBaseUrl(true);
            var imagePath = fileUrl;
            var directoryPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/" + imagePath);
            if (!File.Exists(directoryPath)) return false;
            File.Delete(directoryPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}


