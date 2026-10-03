using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using starterkit.Data.Models.Lab.Schedule;
using starterkit.Data.Services;
using starterkit.Services;

namespace starterkit.Pages.Profile;

public partial class UpdateSchedualModal
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private NotificationService Notification { get; set; } = null!;
    [Inject] private LabService LabService { get; set; } = null!;

    [Parameter] public Schedule? DayData { get; set; }

    // Callback to tell the parent (LabProfile) to refresh the table
    [Parameter] public EventCallback OnSaveSuccess { get; set; }

    private UpdateScheduleCommand _updateScheduleCommand = new();

    // 1. Generate time slots every 30 minutes
    private readonly List<string> _timeOptions = Enumerable.Range(0, 48)
        .Select(i => DateTime.Today.AddMinutes(i * 30).ToString("hh:mm tt"))
        .ToList();

    // 2. Proxy for Opening Time
    private string OpeningTimeProxy
    {
        get => DateTime.Today.Add(_updateScheduleCommand?.OpeningTime ?? TimeSpan.Zero).ToString("hh:mm tt");
        set
        {
            if (DateTime.TryParse(value, out var dt))
            {
                _updateScheduleCommand.OpeningTime = dt.TimeOfDay;
            }
        }
    }

    // 3. Proxy for Closing Time
    private string ClosingTimeProxy
    {
        get => DateTime.Today.Add(_updateScheduleCommand?.ClosingTime ?? TimeSpan.Zero).ToString("hh:mm tt");
        set
        {
            if (DateTime.TryParse(value, out var dt))
            {
                _updateScheduleCommand.ClosingTime = dt.TimeOfDay;
            }
        }
    }


    protected override void OnParametersSet()
    {
        if (DayData != null)
        {
            _updateScheduleCommand = new UpdateScheduleCommand
            {
                Id = DayData.Id,
                OpeningTime = DayData.OpeningTime,
                ClosingTime = DayData.ClosingTime,
                IsOpen = DayData.IsOpen,
                CapacityPerHour = DayData.CapacityPerHour
            };
        }
    }

    private async Task CloseAppointmentModal()
    {
        await JsRuntime.InvokeVoidAsync("HSOverlay.close", "#appointment-modal");
    }

    private async Task HandleUpdateAvailability()
    {
        // Logic check: Closing must be after Opening
        if (_updateScheduleCommand.ClosingTime <= _updateScheduleCommand.OpeningTime)
        {
            await Notification.ShowError("Closing time must be later than opening time.");
            return;
        }

        try
        {
            var success = await LabService.UpdateDayScheduleAsync(_updateScheduleCommand);
            if (success)
            {
                await Notification.ShowToast($"Schedule for '{DayData?.DayName}' updated successfully", 2000);
                await OnSaveSuccess.InvokeAsync(); // Refresh the table in parent
                await CloseAppointmentModal();
            }
            else
            {
                await Notification.ShowError($"Failed to update {DayData?.DayName}. Please try again.");
            }
        }
        catch (Exception)
        {
            await Notification.ShowError("Failed to save changes. Please try again.");
        }
    }
}