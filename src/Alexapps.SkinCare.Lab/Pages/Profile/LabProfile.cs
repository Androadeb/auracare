using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using starterkit.Data.Models.Lab;
using starterkit.Data.Models.Lab.Schedule;
using starterkit.Data.Services;
using starterkit.Services;

namespace starterkit.Pages.Profile;

public partial class LabProfile
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private NotificationService Notification { get; set; } = null!;
    [Inject] private LabService LabService { get; set; } = null!;

    private List<Schedule> _schedule = new();

    private LabInfoResponse? _labData;
    private bool _isLoading = true;
    private bool _isScheduleLoading = true;

    private Schedule? _selectedDay;

    protected override async Task OnInitializedAsync()
    {
        await LoadLabProfileInfo();
        await LoadLabSchedule();
    }

    private async Task LoadLabProfileInfo()
    {
        _isLoading = true;
        try
        {
            var response = await LabService.GetLabInfoAsync();
            if (response is not null)
            {
                _labData = response;
            }
            else
            {
                await Notification.ShowError("Failed to load orders. Please try again.");
            }
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task LoadLabSchedule()
    {
        _isScheduleLoading = true;
        try
        {
            var response = await LabService.GetLabScheduleAsync();
            if (response is not null)
            {
                _schedule = response;
            }
            else
            {
                await Notification.ShowError("Failed to load schedule. Please try again.");
            }
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            _isScheduleLoading = false;
            StateHasChanged();
        }
    }


    private async Task OpenBranchModal()
    {
        await JsRuntime.InvokeVoidAsync("HSOverlay.open", "#add-branch-modal");
    }

    private async Task OpenAppointmentModal(Schedule day)
    {
        _selectedDay = day;
        await JsRuntime.InvokeVoidAsync("HSOverlay.open", "#appointment-modal");
    }
}