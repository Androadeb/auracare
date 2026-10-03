using Alexapps.SkinCare.Blazor.Services;
using AlexApps.Classat.Blazor.Superadmin.Services;
using Blazored.LocalStorage;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Soenneker.Blazor.FilePond;
using Soenneker.Blazor.FilePond.Registrars;
using Soenneker.Blazor.TomSelect;
using Soenneker.Blazor.TomSelect.Registrars;
using ynex.Data;
using ynex.Models.Auth;
using ynex.Services.Auth;
using ynex.Services.Chat;
using ynex.Services.MedicalTest;
using ynex.Services.Patient;
using ynex.Services.Profile;
using ynex.Services.Sessions;
using ynex.Services.Treatment;
using ynex.Services.VideoSessionService;

var builder = WebApplication.CreateBuilder(args);

// --- 1. خدمات Blazor Server الأساسية ---
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
        options.EnableDetailedErrors = true;
    });

// ضروري جداً لخدمة SessionService ولتجنب أخطاء بناء الخدمات
builder.Services.AddHttpContextAccessor();

// --- 2. إعداد الـ Session (مطلوب لـ Blazor Server) ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- 3. خدمات الهوية (Auth & JWT) ---
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddBlazoredLocalStorage();

// تسجيل الخدمات بالترتيب لضمان حل التبعيات
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<RefreshTokenHandler>();
builder.Services.AddScoped<AuthState>();

// --- 4. إعداد الـ HttpClient (الربط مع API) ---
builder.Services.AddHttpClient("DefaultClient", client =>
{
    client.BaseAddress = new Uri("https://auraskin.runasp.net/");
}).AddHttpMessageHandler<RefreshTokenHandler>();

// حقن العميل الافتراضي ليتم استخدامه في كل الخدمات تلقائياً
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("DefaultClient"));

// --- 5. خدمات المشروع (Application Services) ---
builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<AppState>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<IDiagnosticSessionService, DiagnosticSessionService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMedicalTestService, MedicalTestService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<MenuDataService>();
builder.Services.AddScoped<LandingMenuDataService>();
builder.Services.AddScoped<NavScrollService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IVideoSessionService, VideoSessionService>();
// --- 6. الترجمة (Localization) ---
builder.Services.AddLocalization(options => options.ResourcesPath = "Resource/Languages");
builder.Services.AddScoped<JsonLocalizationService>();
builder.Services.AddScoped(typeof(Microsoft.Extensions.Localization.IStringLocalizer<>), typeof(Microsoft.Extensions.Localization.StringLocalizer<>));

// --- 7. مكتبات الواجهة (UI Libraries) ---
builder.Services.AddFilePond();
builder.Services.AddTomSelect();
builder.Services.AddSweetAlert2();
builder.Services.AddWMBOS();
builder.Services.AddWMBSC();

var app = builder.Build();

// --- 8. إعداد Pipeline الطلبات (Middleware) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// إعداد اللغات (العربية افتراضية)
var supportedCultures = new[] { "ar", "en" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.UseStaticFiles();
app.UseRouting();

// تفعيل الـ Session والـ Auth
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();

// الروابط الافتراضية للمشاريع المختلفة
app.MapFallbackToPage("/landing", "/_LandingHost");
app.MapFallbackToPage("/landing-jobs", "/_LandingJobsHost");
app.MapFallbackToPage("{*path:nonfile}", "/_Host");

app.Run();