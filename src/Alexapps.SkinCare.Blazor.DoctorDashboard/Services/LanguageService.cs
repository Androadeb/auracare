using Microsoft.JSInterop;
using System.Globalization;

namespace AlexApps.Classat.Blazor.Superadmin.Services;


/// <summary>
/// Language Service for managing Arabic/English language switching with RTL/LTR support
/// </summary>
public class LanguageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly StateService _stateService;
    private readonly MenuDataService _menuDataService;
    private readonly JsonLocalizationService _jsonLocalizationService;
    private bool _isArabic = true; // Default to Arabic

    public event Action? OnLanguageChanged;

    public bool IsArabic => _isArabic;
    public string Direction => _isArabic ? "rtl" : "ltr";
    public string Lang => _isArabic ? "ar" : "en";

    public LanguageService(IJSRuntime jsRuntime, StateService stateService, MenuDataService menuDataService, JsonLocalizationService jsonLocalizationService)
    {
        _jsRuntime = jsRuntime;
        _stateService = stateService;
        _menuDataService = menuDataService;
        _jsonLocalizationService = jsonLocalizationService;

        // Initialize logic based on current culture if needed
        var culture = CultureInfo.CurrentUICulture.Name;
        if (culture.StartsWith("en"))
        {
            _isArabic = false;
        }
    }

    public async Task SetLanguageAsync(bool isArabic)
    {
        _isArabic = isArabic;
        var cultureCode = isArabic ? "ar-EG" : "en-US";
        var culture = new CultureInfo(cultureCode);

        // Update .NET Culture
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // Update Localization Service
        _jsonLocalizationService.LoadLanguage(cultureCode);

        // Update Menu Data
        _menuDataService.SetLanguage(isArabic);

        // Update direction in state
        //await _stateService.directionFn(isArabic ? "rtl" : "ltr");

        // Update HTML lang attribute
        await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "lang", isArabic ? "ar" : "en");

        // Set the culture cookie for future requests
        var cookieValue = $"c={cultureCode}|uic={cultureCode}";
        await _jsRuntime.InvokeVoidAsync("interop.setCookie", "AspNetCore.Culture", cookieValue, 365);

        // Notify subscribers
        OnLanguageChanged?.Invoke();
    }

    /// <summary>
    /// Initializes the language service from localStorage.
    /// Call this on application startup to restore the user's language preference.
    /// </summary>
    public async Task InitializeFromStorageAsync()
    {
        try
        {
            // Read direction from localStorage
            var storedDirection = await _jsRuntime.InvokeAsync<string>("interop.getLocalStorageItem", "ynexdirection");
            
            if (!string.IsNullOrEmpty(storedDirection))
            {
                _isArabic = storedDirection == "rtl";
                
                // Sync the localization service without full SetLanguageAsync to avoid re-saving
                var cultureCode = _isArabic ? "ar-EG" : "en-US";
                var culture = new CultureInfo(cultureCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                _jsonLocalizationService.LoadLanguage(cultureCode);
                _menuDataService.SetLanguage(_isArabic);
                
                // Update HTML lang attribute
                await _jsRuntime.InvokeVoidAsync("interop.addAttributeToHtml", "lang", _isArabic ? "ar" : "en");
                
                // Sync with state service
                //await _stateService.directionFn(_isArabic ? "rtl" : "ltr");
                
                // Notify subscribers
                OnLanguageChanged?.Invoke();
            }
        }
        catch
        {
            // Silently fail if localStorage is not accessible (e.g., prerendering)
        }
    }

    public async Task ToggleLanguageAsync()
    {
        await SetLanguageAsync(!_isArabic);
    }

    // Helper method to get localized text
    public string GetText(string arabicText, string englishText)
    {
        return _isArabic ? arabicText : englishText;
    }
}