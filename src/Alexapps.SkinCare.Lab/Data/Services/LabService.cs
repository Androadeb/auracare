using starterkit.Data.Models.Lab;
using starterkit.Data.Models.Lab.Schedule;

namespace starterkit.Data.Services;

public class LabService(IHttpClientFactory httpClientFactory)
{
    public async Task<LabInfoResponse?> GetLabInfoAsync()
    {
        var httpClient = httpClientFactory.CreateClient("ServerAPI");
        var response = await httpClient.GetAsync("api/v1/auth/my-info");

        return await response.Content.ReadFromJsonAsync<LabInfoResponse>();
    }

    public async Task<List<Schedule>?> GetLabScheduleAsync()
    {
        var httpClient = httpClientFactory.CreateClient("ServerAPI");
        var response = await httpClient.GetAsync("api/v1/lab/schedules/my-schedule");

        return await response.Content.ReadFromJsonAsync<List<Schedule>>();
    }

    public async Task<bool> UpdateDayScheduleAsync(UpdateScheduleCommand scheduleCommand)
    {
        var httpClient = httpClientFactory.CreateClient("ServerAPI");
        var payload = new
        {
            Schedules = new List<UpdateScheduleCommand>
            {
                scheduleCommand
            }
        };
        var response = await httpClient.PutAsJsonAsync("api/v1/lab/schedules/bulk-update", payload);

        return response.IsSuccessStatusCode;
    }
}