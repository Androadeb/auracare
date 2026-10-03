using starterkit.Data.Models.MedicalTests;

namespace starterkit.Data.Services;

public class MedicalTestService(IHttpClientFactory httpClientFactory)
{
	public async Task<MedicalTestApiResponse?> GetTestsAsync(int page, int limit, string? search = null,
		bool? isEnabled = null)
	{
		var httpClient = httpClientFactory.CreateClient("ServerAPI");
		var url = $"api/v1/lab/medical-tests?page={page}&limit={limit}";

		if (!string.IsNullOrWhiteSpace(search))
		{
			url += $"&filter={Uri.EscapeDataString(search)}";
		}

		if (isEnabled.HasValue)
		{
			url += $"&isEnabled={isEnabled.Value.ToString().ToLower()}";
		}

		return await httpClient.GetFromJsonAsync<MedicalTestApiResponse>(url);
	}

	public async Task<List<MedicalTestCategory>> GetCategoriesAsync()
	{
		var httpClient = httpClientFactory.CreateClient("ServerAPI");
		var response = await httpClient.GetAsync("api/v1/medical-tests/categories");

		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<List<MedicalTestCategory>>() ?? [];
		}

		return [];
	}

	public async Task<List<MedicalTest>> GetTestsByCategoryAsync(string categoryId)
	{
		var httpClient = httpClientFactory.CreateClient("ServerAPI");
		var response =
			await httpClient.GetFromJsonAsync<List<MedicalTest>>(
				$"api/v1/medical-tests/categories/{categoryId}/tests");

		return response ?? new List<MedicalTest>();
	}

	public async Task<bool> CreateTestAsync(CreateMedicalTest test)
	{
		var httpClient = httpClientFactory.CreateClient("ServerAPI");
		var response = await httpClient.PostAsJsonAsync("api/v1/lab/medical-tests", test);

		return response.IsSuccessStatusCode;
	}

	public async Task<bool> UpdateTestAsync(string id, CreateMedicalTest model)
	{
		var httpClient = httpClientFactory.CreateClient("ServerAPI");
		var response = await httpClient.PutAsJsonAsync($"api/v1/lab/medical-tests/{id}", model);

		return response.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteTestAsync(string id)
	{
		var httpClient = httpClientFactory.CreateClient("ServerAPI");
		var response = await httpClient.DeleteAsync($"api/v1/lab/medical-tests/{id}");

		return response.IsSuccessStatusCode;
	}
}

