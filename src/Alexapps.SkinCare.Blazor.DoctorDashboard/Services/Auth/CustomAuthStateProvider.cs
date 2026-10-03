using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using ynex.Models.Auth;

namespace ynex.Services.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedUser = await _localStorage.GetItemAsync<LoginResponse>("user_data");

                if (savedUser == null || string.IsNullOrWhiteSpace(savedUser.AccessToken))
                {
                    return new AuthenticationState(_anonymous);
                }

                // Set the default Authorization header for the first time
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedUser.AccessToken);

                return new AuthenticationState(CreateClaimsPrincipal(savedUser.AccessToken));
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task NotifyUserAuthentication(LoginResponse user)
        {
            await _localStorage.SetItemAsync("user_data", user);
            var authenticatedUser = CreateClaimsPrincipal(user.AccessToken);
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task NotifyUserLogout()
        {
            await _localStorage.RemoveItemAsync("user_data");
            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(string jwtToken)
        {

            var identity = new ClaimsIdentity(
                ParseClaimsFromJwt(jwtToken),
                "jwt",
                "name",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            return new ClaimsPrincipal(identity);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    // Handle roles if they are in an array
                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in element.EnumerateArray())
                        {
                            claims.Add(new Claim(kvp.Key, item.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                    }
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}