using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System; // Required for Exception handling

public class SessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Helper method to safely check session availability
    private bool IsSessionAvailable()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            return context != null && context.Session != null && context.Session.IsAvailable;
        }
        catch { return false; }
    }

    public async Task SetAppStateToSession(AppState state)
    {
        try
        {
            if (IsSessionAvailable())
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var jsonState = JsonSerializer.Serialize(state);
                session.SetString("AppState", jsonState);
            }
        }
        catch (InvalidOperationException)
        {
            // Catching: The session cannot be established after the response has started.
            // In Blazor Server, this is common and should be handled silently.
        }

        await Task.CompletedTask;
    }

    public void DeleteAppStateFromSession()
    {
        try
        {
            if (IsSessionAvailable())
            {
                _httpContextAccessor.HttpContext.Session.Remove("AppState");
            }
        }
        catch { /* Silent fail to prevent circuit crash */ }
    }

    public Task<AppState> GetAppStateFromSession()
    {
        try
        {
            if (!IsSessionAvailable()) return Task.FromResult(new AppState());

            var session = _httpContextAccessor.HttpContext.Session;
            var jsonState = session.GetString("AppState");

            if (string.IsNullOrEmpty(jsonState)) return Task.FromResult(new AppState());

            return Task.FromResult(JsonSerializer.Deserialize<AppState>(jsonState) ?? new AppState());
        }
        catch
        {
            return Task.FromResult(new AppState());
        }
    }

    public async Task SetInitalAppStateToSession(AppState state)
    {
        try
        {
            if (IsSessionAvailable())
            {
                var session = _httpContextAccessor.HttpContext.Session;
                var jsonState = JsonSerializer.Serialize(state);
                session.SetString("InitalAppState", jsonState);
            }
        }
        catch { /* Handle session timing issue */ }

        await Task.CompletedTask;
    }

    public async Task<AppState?> GetInitalAppStateFromSession()
    {
        try
        {
            if (!IsSessionAvailable()) return null;

            var session = _httpContextAccessor.HttpContext.Session;
            var jsonState = session.GetString("InitalAppState");

            if (string.IsNullOrEmpty(jsonState)) return null;

            return JsonSerializer.Deserialize<AppState>(jsonState);
        }
        catch
        {
            return null;
        }
    }
}