using System.Globalization;
using System.Text;
using System.Text.Json;

namespace AlexApps.Classat.Blazor.Superadmin.Services;

public class JsonLocalizationService
{
    private readonly IHostEnvironment _hostEnvironment;
    private Dictionary<string, string> _localizedStrings;
    private string _currentCulture;

    public JsonLocalizationService(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
        _localizedStrings = new Dictionary<string, string>();
    }

    public string this[string key]
    {
        get
        {
            // «·Õ’Ê· ⁄·Ï ar √Ê en ›ﬁÿ
            var culture = CultureInfo.CurrentUICulture.Name.Split('-')[0].ToLower();

            if (_currentCulture != culture || _localizedStrings.Count == 0)
            {
                LoadLanguage(culture);
            }

            if (_localizedStrings.TryGetValue(key, out var value))
            {
                return value;
            }

            return key;
        }
    }

    
    public void LoadLanguage(string languageCode)
    {
     
        var shortLang = languageCode.Split('-')[0].ToLower();
        _currentCulture = shortLang;

        try
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "Resource", "Languages", $"{shortLang}.json");

           
            Console.WriteLine($"Looking for language file at: {filePath}");

            if (File.Exists(filePath))
            {
                
                var jsonContent = File.ReadAllText(filePath, Encoding.UTF8);

                _localizedStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent)
                                    ?? new Dictionary<string, string>();
            }
        }
        catch (Exception ex) { /* log error */ }
    }
}