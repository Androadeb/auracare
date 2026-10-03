using System.Text.Json.Serialization;

namespace ynex.Models.Blog
{
    public class BlogModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        // هذا الحقل المسؤول عن استلام البيانات من الـ API
        [JsonPropertyName("subtitle")]
        public string ShortDescription { get; set; } = string.Empty;

        // --- أضف هذا الجزء لحل مشكلة الكومبيلر في الـ Razor ---
        [JsonIgnore] // نخبر السيرياليزر بتجاهله لأنه مجرد "مرآة"
        public string Subtitle
        {
            get => ShortDescription;
            set => ShortDescription = value;
        }
        // ----------------------------------------------------

        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("imageUrl")]
        public string? CoverImage { get; set; }

        // يفضل تحديد نوعه أو إعطاؤه قيمة افتراضية
        public object Status { get; set; } = "0";

        [JsonPropertyName("creationTime")]
        public DateTime PublishDate { get; set; }

        public int ViewsCount { get; set; }
        public int LikeCount { get; set; }

        public string AuthorName { get; set; } = "Admin";

        [JsonPropertyName("authorImage")]
        public string? AuthorAvatar { get; set; }

        public string FullCoverImage => BuildUrl(CoverImage, "https://ui-avatars.com/api/?name=Blog&background=F1A68E");

        public string FullAuthorAvatar => BuildUrl(AuthorAvatar, "https://ui-avatars.com/api/?name=Admin&background=F1A68E&color=fff");
        public Guid? SpecialtyId { get; set; }
        public string? SpecialtyName { get; set; }
        private string BuildUrl(string? path, string defaultUrl)
        {
            if (string.IsNullOrWhiteSpace(path)) return defaultUrl;

            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase) || path.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return path;

            var baseUrl = "https://auraskin.runasp.net";

            return $"{baseUrl.TrimEnd('/')}/{(path.TrimStart('/'))}";
        }
    }
}