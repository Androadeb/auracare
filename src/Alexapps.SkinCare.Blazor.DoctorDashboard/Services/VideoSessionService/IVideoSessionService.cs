using ynex.Models.ScheduledVideo;

namespace ynex.Services.VideoSessionService
{
    public interface IVideoSessionService
    {
        Task<VideoDashboardResponse> GetDashboardDataAsync(int page = 1, int limit = 10, bool? isCompleted = null);
    }
}
