using Alexapps.SkinCare.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.Notifications
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
        public string TargetType { get; set; }
        public Guid? TargetId { get; set; }
        public Notification()
        {
        }

    }
}
