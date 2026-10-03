using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using starterkit.Data.Services;
using starterkit.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

var enCulture = new CultureInfo("en-US")
{
	DateTimeFormat = { Calendar = new GregorianCalendar() }
};

var arCulture = new CultureInfo("ar-SA")
{
	DateTimeFormat = { Calendar = new UmAlQuraCalendar() }
};

var supportedCultures = new[] { enCulture, arCulture };
// Add services to the container.
builder.Services.AddLocalization();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
	options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(30);
});
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<StateService>();
builder.Services.AddSingleton<AppState>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<MenuDataService>();
builder.Services.AddWMBOS();
builder.Services.AddScoped<NavScrollService>();
builder.Services.AddSession();
builder.Services.AddScoped<SessionService>();

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<MedicalTestService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<LabService>();

builder.Services.AddSingleton<ITokenProvider, InMemoryTokenProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());


builder.Services.AddHttpClient("ServerAPI", client =>
	client.BaseAddress = new Uri(apiBaseUrl ?? "http://localhost:4200/")
).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient("AuthClient", client =>
	client.BaseAddress = new Uri(apiBaseUrl ?? "http://localhost:4200/"));

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.LoginPath = "/login";
//         options.AccessDeniedPath = "/accessdenied";
//         options.Cookie.Name = "YourAppCookieName";
//         options.Cookie.HttpOnly = true;
//         options.Cookie.SameSite = SameSiteMode.Strict;
//         options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//     });


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSession();

app.UseStaticFiles();

app.UseRequestLocalization(options =>
{
	options.DefaultRequestCulture =
		new RequestCulture(enCulture);

	options.SupportedCultures = supportedCultures;
	options.SupportedUICultures = supportedCultures;
});

app.UseRouting();

app.MapBlazorHub();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToPage("/_Host");

app.Run();
