using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alexapps.SkinCare.Dtos.Blogs.Commands
{
    public class CreateUpdateBlogDto
    {
        [Required]
        
        public string Title { get; set; }

       
        public string Subtitle { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile ImageFile { get; set; }

   
    }
}
