using Microsoft.AspNetCore.Components.Forms;
using starterkit.Data.Models.Orders;

namespace starterkit.Data.Services;

public class OrderService(IHttpClientFactory httpClientFactory)
{
  public async Task<OrderApiResponse?> GetOrdersAsync(int page, int limit, string? search = null,
    DiagnosticTestStatus? Status = null, LabServiceType? labServiceType = null, DateTime? requestDate = null)
  {
    var httpClient = httpClientFactory.CreateClient("ServerAPI");
    var url = $"api/v1/lab/diagnostic-session-tests/orders?page={page}&limit={limit}";

    if (!string.IsNullOrWhiteSpace(search))
    {
      url += $"&filter={Uri.EscapeDataString(search)}";
    }

    // if (Status.HasValue) url += $"&status={Status.Value.ToString().ToLower()}";
    if (labServiceType.HasValue)
    {
      url += $"&servicetype={labServiceType}";
    }

    if (requestDate.HasValue)
    {
      url += $"&requestdate={requestDate.Value:yyyy/MM/dd}";
    }

    return await httpClient.GetFromJsonAsync<OrderApiResponse>(url);
  }

  public async Task<Order?> GetOrderDetailsAsync(string id)
  {
    var url = $"api/v1/lab/diagnostic-session-tests/{id}/details";

    var httpClient = httpClientFactory.CreateClient("ServerAPI");
    return await httpClient.GetFromJsonAsync<Order>(url);
  }

  public async Task<bool> UpdateOrderStatusAsync(string id, string status)
  {
    var url = $"api/v1/lab/diagnostic-session-tests/{id}/status";

    var httpClient = httpClientFactory.CreateClient("ServerAPI");
    var response = await httpClient.PutAsJsonAsync(url, new { newStatus = status });

    return response.IsSuccessStatusCode;
  }

  public async Task<ResultFileResponse?> UploadOrderResultAsync(string orderId, IBrowserFile file)
  {
    var httpClient = httpClientFactory.CreateClient("ServerAPI");
    using var content = new MultipartFormDataContent();
    var fileContent = new StreamContent(file.OpenReadStream(1024 * 1024 * 5)); // 5MB max
    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

    content.Add(fileContent, "file", file.Name);
    content.Add(new StringContent(orderId), "orderId");

    var response =
      await httpClient.PostAsync($"api/v1/lab/diagnostic-session-tests/{orderId}/upload-result", content);
    return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ResultFileResponse>() : null;
  }
}
