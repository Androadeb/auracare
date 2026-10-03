using Alexapps.SkinCare.Entities.Base;
using Alexapps.SkinCare.Entities.Doctors;
using Alexapps.SkinCare.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Entities.LABs
{
    public class Lab : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public bool IsActive { get; set; }

      
        public ICollection<LabBranch> Branches { get; set; }
    }
}
