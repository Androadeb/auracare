using System.Net;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using starterkit.Data.Models.Auth;

namespace starterkit.Services
{
  public class AuthHeaderHandler : DelegatingHandler
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly CustomAuthStateProvider _authStateProvider;

    public AuthHeaderHandler(
      ITokenProvider tokenProvider,
      IHttpClientFactory httpClientFactory,
      ILocalStorageService localStorage,
      CustomAuthStateProvider authStateProvider)
    {
      _tokenProvider = tokenProvider;
      _httpClientFactory = httpClientFactory;
      _localStorage = localStorage;
      _authStateProvider = authStateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      var accessToken = _tokenProvider.AccessToken;
      if (!string.IsNullOrEmpty(accessToken))
      {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
      }

      var response = await base.SendAsync(request, cancellationToken);

      if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(_tokenProvider.RefreshToken))
      {
        // Avoid refreshing if this request IS the refresh request or logout request
        if (request.RequestUri?.AbsolutePath.Contains("refresh-token") == true)
        {
          return response;
        }

        var refreshResult = await RefreshAccessToken(_tokenProvider.RefreshToken);
        if (refreshResult != null)
        {
          _tokenProvider.AccessToken = refreshResult.AccessToken;
          _tokenProvider.RefreshToken = refreshResult.RefreshToken;
          try
          {
            await _localStorage.SetItemAsync("authToken", refreshResult.AccessToken, cancellationToken);
            await _localStorage.SetItemAsync("refreshToken", refreshResult.RefreshToken, cancellationToken);
          }
          catch (Exception ex)
          {
            // Log or ignore: JS might not be ready yet
            Console.WriteLine($"LocalStorage sync failed: {ex.Message}");
          }

          request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.AccessToken);

          return await base.SendAsync(request, cancellationToken);
          ;
        }
      }

      return response;
    }

    private async Task<AuthResponse?> RefreshAccessToken(string refreshToken)
    {
      var client = _httpClientFactory.CreateClient("AuthClient");
      var response = await client.PostAsJsonAsync("api/v1/auth/refresh-token", new { RefreshToken = refreshToken });
      return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<AuthResponse>() : null;
    }
  }
}
