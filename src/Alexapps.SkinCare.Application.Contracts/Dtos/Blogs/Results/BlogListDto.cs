using Alexapps.SkinCare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Alexapps.SkinCare.Dtos.Blogs.Results
{
    public class BlogListDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public BlogStatus Status { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImage { get; set; } 
        public DateTime CreationTime { get; set; }

     
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
    }
}
