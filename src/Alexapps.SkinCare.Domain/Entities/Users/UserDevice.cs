using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Alexapps.SkinCare.Entities.Users
{
    public class UserDevice : CreationAuditedEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string Language { get; set; }
        public bool IsActive { get; set; }
        protected UserDevice() { }

        public UserDevice(Guid id, Guid userId, string token, string language, bool isActive = true)
            : base(id)
        {
            UserId = userId;
            Token = token;
            Language = language;
         
            IsActive = isActive;
        }
    }
}
