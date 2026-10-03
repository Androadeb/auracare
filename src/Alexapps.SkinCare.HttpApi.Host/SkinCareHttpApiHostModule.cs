using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Alexapps.SkinCare.MultiTenancy;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Swashbuckle;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.FluentValidation;
using Alexapps.SkinCare.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Globalization;
using Alexapps.SkinCare.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Alexapps.SkinCare.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp.AspNetCore.SignalR;
using Alexapps.SkinCare.Hubs;
using Volo.Abp.OpenIddict;
using OpenIddict.Server;
using System.Security.Cryptography.X509Certificates;
namespace Alexapps.SkinCare;

[DependsOn(
    typeof(SkinCareHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(SkinCareApplicationModule),
    typeof(SkinCareEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
[DependsOn(
    typeof(AbpFluentValidationModule),
    typeof(AbpAspNetCoreSignalRModule)
)]
public class SkinCareHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("SkinCare");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            var certificatePath = Path.Combine(
                hostingEnvironment.ContentRootPath,
                "openiddict.pfx"
            );

            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                var certificate = X509CertificateLoader.LoadPkcs12FromFile(
                    certificatePath,
                    configuration["AuthServer:CertificatePassPhrase"],
                    X509KeyStorageFlags.MachineKeySet |
                    X509KeyStorageFlags.EphemeralKeySet
                );

                serverBuilder
                    .AddEncryptionCertificate(certificate)
                    .AddSigningCertificate(certificate);
            });
        }

        var usersConfig = new UsersFeaturesConfigurations();
        configuration.Bind(UsersFeaturesConfigurations.SectionName, usersConfig);

        var jwtConfig = new JwtConfiguration();
        configuration.Bind(JwtConfiguration.SectionName, jwtConfig);

        var storageConfig = new StorageConfiguration();
        configuration.Bind(StorageConfiguration.SectionName, storageConfig);

        context.Services.AddSingleton(usersConfig);
        context.Services.AddSingleton(jwtConfig);
        context.Services.AddSingleton(storageConfig);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        ConfigureAuthentication(context);
        ConfigureVirtualFileSystem(context);
        ConfigureCors(context, configuration);

        //
        ConfigureSwagger(context, configuration);
        ConfigureJwt(context, configuration);

        Configure<IdentityOptions>
            (options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = true;
            });

        var services = context.Services;
        services.Configure<MvcOptions>(options =>
        {
            // Register the custom exception filter
            options.Filters.Add(new Filters.CustomUserFriendlyExceptionFilter());
        });
    }

    private void ConfigureJwt(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? "default_jwt_key";

        context.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                };
                // Add the event handler
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // Prevent the default response
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        context.Response.WriteAsync("{\"error\": \"Forbidden\"}");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        // Check if the request is for SignalR and has an access_token in the query string
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chat") || path.StartsWithSegments("/notification") || path.StartsWithSegments("/signalr-hubs")))
                        {
                            // Read the token from the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkinCare API - Shared", Version = "v1" });
            c.SwaggerDoc("admin", new OpenApiInfo { Title = "SkinCare API - Admin", Version = "v1" });
            c.SwaggerDoc("client", new OpenApiInfo { Title = "SkinCare API - Client", Version = "v1" });
            c.SwaggerDoc("doctor", new OpenApiInfo { Title = "SkinCare API - Doctor", Version = "v1" });
            c.SwaggerDoc("lab", new OpenApiInfo { Title = "SkinCare API - Lab", Version = "v1" });

            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                var relativePath = apiDesc.RelativePath?.ToLowerInvariant();
                if (string.IsNullOrEmpty(relativePath)) return false;

                if (docName == "admin") return relativePath.Contains("/admin/");
                if (docName == "client") return relativePath.Contains("/client/")
                                                || relativePath.Contains("/doctors")
                                                || relativePath.Contains("/home-services")
                                                || relativePath.Contains("/home-service-sessions")
                                                || (relativePath.Contains("/diagnostic-sessions") && !relativePath.Contains("/doctor/"));

                if (docName == "doctor") return relativePath.Contains("/doctor/");
                if (docName == "lab") return relativePath.Contains("/lab/");
                if (docName == "v1") return relativePath.Contains("/auth/") || relativePath.Contains("pages") || relativePath.Contains("/skin-conditions") || relativePath.Contains("/medical-tests") || (relativePath.Contains("/medications") && !relativePath.Contains("/admin/"));

                return false;
            });

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            c.SchemaFilter<EnumSchemaFilter>();
            c.OperationFilter<AcceptLanguageHeaderOperationFilter>();

            // Optional: Include XML comments (if you have them)
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
    }


    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<SkinCareDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Alexapps.SkinCare.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<SkinCareDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Alexapps.SkinCare.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<SkinCareApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Alexapps.SkinCare.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<SkinCareApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Alexapps.SkinCare.Application"));
            });
        }
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                    {"SkinCare", "SkinCare API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "SkinCare API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/"))
                        .ToArray() ?? Array.Empty<string>())
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }
        app.UseAbpRequestLocalization(options =>
        {
            // Change the order of RequestCultureProviders to prioritize Accept-Language
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                    new AcceptLanguageHeaderRequestCultureProvider(),  //  Accept-Language Header
                    new QueryStringRequestCultureProvider(),           //  Query String (`?culture=ar`)
                    new CookieRequestCultureProvider()                 //  Cookie (if stored from a previous visit)
            };
        });

        //  Force numbers to always be formatted in English
        CultureInfo englishNumberFormat = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = englishNumberFormat;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseAbpSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shared API");
            c.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
            c.SwaggerEndpoint("/swagger/client/swagger.json", "Client API");
            c.SwaggerEndpoint("/swagger/doctor/swagger.json", "Doctor API");
            c.SwaggerEndpoint("/swagger/lab/swagger.json", "Lab API");

            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            c.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            c.OAuthScopes("SkinCare");
        });

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapHub<DiagnosticSessionHub>("/signalr-hubs/diagnostic-sessions");
        });
    }
    public class AcceptLanguageHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "Language preference for the response (e.g., en-US, ar, etc.)",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}



