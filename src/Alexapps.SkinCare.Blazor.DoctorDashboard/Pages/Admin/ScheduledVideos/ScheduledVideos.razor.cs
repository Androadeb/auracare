using Alexapps.SkinCare.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ynex.Models.ScheduledVideo;
using ynex.Services.VideoSessionService;
using ScheduledVideoModel = ynex.Models.ScheduledVideo.ScheduledVideo;

namespace Alexapps.SkinCare.Blazor.Pages.Admin
{
    public partial class ScheduledVideos : ComponentBase
    {
        [Inject] private IVideoSessionService VideoService { get; set; }
        [Inject] private IJSRuntime JS { get; set; }


        public PaginationMetadata Metadata { get; set; } 
        protected List<ScheduledVideoModel> ScheduledCalls { get; set; } = new();
        protected List<StatisticItem> StatisticsList { get; set; } = new();
        protected bool isMeetingActive { get; set; } = false;

        // variables for control
        protected bool ShowCompleted { get; set; } = false; // Default filter: Upcoming
        protected int CurrentPage { get; set; } = 1;
        protected int TotalPages { get; set; } = 1;
        protected bool HasNextPage { get; set; }
        protected bool HasPreviousPage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
               
                if (CurrentPage < 1) CurrentPage = 1;

              
                var response = await VideoService.GetDashboardDataAsync(CurrentPage, 8, ShowCompleted);

                if (response != null && response.Appointments != null)
                {
                   
                    ScheduledCalls = response.Appointments.Items ?? new List<ScheduledVideoModel>();

                  
                    if (response.Appointments.Metadata != null)
                    {
                        var meta = response.Appointments.Metadata;

                        CurrentPage = meta.Page <= 0 ? 1 : meta.Page;

                        TotalPages = meta.TotalPages;
                        HasNextPage = meta.HasNextPage;
                        HasPreviousPage = meta.HasPreviousPage;
                    }

                    MapStatistics(response);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        protected async Task StartMeeting(ScheduledVideoModel call)
        {
            if (string.IsNullOrEmpty(call.RoomId) || string.IsNullOrEmpty(call.Token))
            {
                Console.WriteLine("Meeting data is missing (RoomId or Token).");
                return;
            }

            // 1. Activate meeting UI
            isMeetingActive = true;
            StateHasChanged();

            // 2. Short delay to ensure DOM is rendered
            await Task.Delay(500);

            try
            {
                // Prepare translated error message for JS
                string cameraErrorMsg = Loc["Please enable camera and microphone from browser settings."];

                // 3. Call VideoSDK init via JS
                await JS.InvokeVoidAsync("videoSDKHandler.init", call.RoomId, call.Token, "Doctor", cameraErrorMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching video call: {ex.Message}");
            }
        }

        // Tab switching logic
        protected async Task FilterSessions(bool completed)
        {
            ShowCompleted = completed;
            CurrentPage = 1; // Reset to page 1 on filter change
            await LoadDataAsync();
        }

        protected async Task ChangePage(int newPage)
        {
            
            if (newPage < 1) return;
            if (TotalPages > 0 && newPage > TotalPages) return;

            CurrentPage = newPage;
            await LoadDataAsync();
        }

        private void MapStatistics(VideoDashboardResponse data)
        {
            StatisticsList = new List<StatisticItem>
    {
        new StatisticItem {
            TitleKey = "Today's Calls", // تأكد من وجود هذا المفتاح في ar.json
            Value = data.TodayCallsCount.ToString(),
            IconClass = "ri-video-chat-line",
            TextColor = "text-green-500",
            BgColorOpacity = "bg-green-500/10",
            GlowColor = "rgba(34, 197, 94, 0.4)"
        },
        new StatisticItem {
            TitleKey = "Last 7 Days",
            Value = data.WeeklySessionsCount.ToString(),
            SubTitleKey = "Total sessions",
            IconClass = "ri-calendar-check-line",
            TextColor = "text-orange-400",
            BgColorOpacity = "bg-orange-500/10",
            GlowColor = "rgba(251, 146, 60, 0.4)"
        },
        new StatisticItem {
            TitleKey = "Avg Duration",
            Value = data.AvgDuration ?? "0m",
            SubTitleKey = "Average time",
            IconClass = "ri-time-line",
            TextColor = "text-purple-500",
            BgColorOpacity = "bg-purple-500/10",
            GlowColor = "rgba(168, 85, 247, 0.4)"
        }
    };
        }
    }
}