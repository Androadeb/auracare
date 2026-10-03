using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Notifications
{
    public class SendNotificationArgs
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
