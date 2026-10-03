using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.DiagnosticSessions.Commands
{
    public class VideoSdkCreateRoomRequest
    {
        public string customRoomId { get; set; }
        public AutoCloseConfig autoCloseConfig { get; set; }
    }

    public class AutoCloseConfig
    {
        public string type { get; set; } = "session-end-and-deactivate";
        public int duration { get; set; } = 60; 
    }
}
