using System;
using System.Collections.Generic;

namespace ynex.Models.ScheduledVideo
{
    public class VideoDashboardResponse
    {
        public int TodayCallsCount { get; set; }
        public int WeeklySessionsCount { get; set; }
        public string AvgDuration { get; set; }
        public AppointmentList Appointments { get; set; }

        // تم إضافة الميتا داتا هنا لتعمل عملية الـ Pagination بشكل صحيح
      
    }

    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class AppointmentList
    {
        public List<ScheduledVideo> Items { get; set; }

        public PaginationMetadata Metadata { get; set; }
    }

    public class ScheduledVideo
    {
        public Guid SessionId { get; set; }
        public string PatientName { get; set; }
        public string PatientImageUrl { get; set; }
        public string ConsultationType { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool CanStart { get; set; }
        public bool IsCompleted { get; set; }
        public string RoomId { get; set; }
        public string Token { get; set; }

        // خاصية مساعدة لتنسيق الوقت في الواجهة
        public string FormattedTime => ScheduledStartTime.ToString("hh:mm tt");
    }

    public class StatisticItem
    {
        public string TitleKey { get; set; }
        public string Value { get; set; }
        public int Percentage { get; set; }
        public string SubTitleKey { get; set; }
        public string IconClass { get; set; }
        public string TextColor { get; set; }
        public string BgColorOpacity { get; set; }
        public string GlowColor { get; set; }
    }
}