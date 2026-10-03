using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alexapps.SkinCare.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using Alexapps.SkinCare.Configurations;

namespace Alexapps.SkinCare;

/* Inherit your application services from this class.
 */
public abstract class SkinCareAppService : ApplicationService
{
    protected IHttpContextAccessor HttpContextAccessor => LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();
    protected IConfiguration Configuration => LazyServiceProvider.LazyGetRequiredService<IConfiguration>();
    protected StorageConfiguration StorageConfiguration => LazyServiceProvider.LazyGetRequiredService<StorageConfiguration>();

    protected SkinCareAppService()
    {
        LocalizationResource = typeof(SkinCareResource);
    }

    protected string GetBaseUrl()
    {
        var baseUrl = StorageConfiguration?.BaseUrl;
        
        if (string.IsNullOrEmpty(baseUrl))
        {
             baseUrl = Configuration?["App:SelfUrl"];
        }
        
        if (string.IsNullOrEmpty(baseUrl) && HttpContextAccessor?.HttpContext != null)
        {
            var request = HttpContextAccessor.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host}";
        }

        return baseUrl ?? "";
    }
}



