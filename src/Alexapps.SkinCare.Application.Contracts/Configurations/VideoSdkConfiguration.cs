using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Configurations
{
    public class VideoSdkConfiguration
    {
        public const string SectionName = "VideoSdk";
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string ApiBaseUrl { get; set; } = "https://api.videosdk.live/v2/";
    }
}
