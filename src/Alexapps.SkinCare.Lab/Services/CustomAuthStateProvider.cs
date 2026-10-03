
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace starterkit.Services
{
	public class CustomAuthStateProvider(ITokenProvider tokenProvider) : AuthenticationStateProvider
	{
		private readonly ITokenProvider _tokenProvider = tokenProvider;

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = _tokenProvider.AccessToken;

			// 1. Handle Anonymous state safely
			if (string.IsNullOrWhiteSpace(token))
			{
				return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
			}

			try
			{
				var claims = ParseClaimsFromJwt(token);
				var identity = new ClaimsIdentity(claims, "jwt");
				return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
			}
			catch
			{
				// If token is malformed, return anonymous
				return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
			}
		}

		public void NotifyUserAuthentication(string token)
		{
			_tokenProvider.AccessToken = token;
			var authState = GetAuthenticationStateAsync();
			NotifyAuthenticationStateChanged(authState);
		}

		public void NotifyUserLogout()
		{
			_tokenProvider.AccessToken = null;
			_tokenProvider.RefreshToken = null;
			var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
		}

		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			if (string.IsNullOrWhiteSpace(jwt) || !jwt.Contains('.'))
			{
				return [];
			}

			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

			if (keyValuePairs == null)
			{
				return [];
			}

			var claims = new List<Claim>();


			foreach (var kvp in keyValuePairs)
			{
				// Robust Role Handling (handles single string or array)
				if (kvp.Key == ClaimTypes.Role || kvp.Key == "role")
				{
					if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
					{
						foreach (var item in element.EnumerateArray())
						{
							claims.Add(new Claim(ClaimTypes.Role, item.ToString()));
						}
					}
					else
					{
						claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString()!));
					}
				}
				else
				{
					claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
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
