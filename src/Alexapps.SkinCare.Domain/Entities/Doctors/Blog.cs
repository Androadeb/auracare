using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Alexapps.SkinCare.Entities.Doctors
{
    public class Blog : FullAuditedAggregateRoot<Guid>
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public BlogStatus Status { get; set; } = BlogStatus.Pending;
        public Guid? AuthorId { get; set; }
        public Doctor Author { get; set; }
        public Guid? LastStatusChangedBy { get; set; } 
      
    }
}
