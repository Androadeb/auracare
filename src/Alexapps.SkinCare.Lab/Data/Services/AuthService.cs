using Blazored.LocalStorage;
using starterkit.Data.Models.Auth;
using starterkit.Services;

namespace starterkit.Data.Services;

public class AuthService(
	IHttpClientFactory httpClientFactory,
	CustomAuthStateProvider authStateProvider,
	ILocalStorageService localStorage,
	ITokenProvider tokenProvider)
{
	public async Task<bool> SignInWithEmailAsync(LoginRequest model)
	{
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var response = await httpClient.PostAsJsonAsync("api/v1/auth/sign-in-otp", model);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> SignInWithPhoneAsync(LoginWithPhoneRequest model)
	{
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var response = await httpClient.PostAsJsonAsync("api/v1/auth/send-otp", model);
		return response.IsSuccessStatusCode;
	}

	public async Task<bool> VerifyOtpAsync(OtpRequest model)
	{
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var response = await httpClient.PostAsJsonAsync("api/v1/auth/verify-otp", model);

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

			if (result is not null)
			{
				tokenProvider.AccessToken = result.AccessToken;
				tokenProvider.RefreshToken = result.RefreshToken;

				await localStorage.SetItemAsync("authToken", result.AccessToken);
				await localStorage.SetItemAsync("refreshToken", result.RefreshToken);

				authStateProvider.NotifyUserAuthentication(result.AccessToken);

				return true;
			}
		}

		return false;
	}

	public async Task<bool> ResendOtpAsync(ResendOtpRequest modelResendOtpRequest)
	{
		var httpClient = httpClientFactory.CreateClient("AuthClient");
		var response = await httpClient.PostAsJsonAsync("api/v1/auth/send-otp", modelResendOtpRequest);
		return response.IsSuccessStatusCode;
	}

	public async Task LogoutAsync()
	{
		tokenProvider.AccessToken = null;
		tokenProvider.RefreshToken = null;

		try
		{
			await localStorage.RemoveItemAsync("authToken");
			await localStorage.RemoveItemAsync("refreshToken");
		}
		catch (InvalidOperationException)
		{
		}

		authStateProvider.NotifyUserLogout();
	}

	public async Task InitializeAsync()
	{
		try
		{
			var token = await localStorage.GetItemAsync<string>("authToken");
			var refresh = await localStorage.GetItemAsync<string>("refreshToken");

			if (!string.IsNullOrEmpty(token))
			{
				tokenProvider.AccessToken = token;
				tokenProvider.RefreshToken = refresh;
				authStateProvider.NotifyUserAuthentication(token);
			}
		}
		catch (InvalidOperationException)
		{
		}
	}
}

