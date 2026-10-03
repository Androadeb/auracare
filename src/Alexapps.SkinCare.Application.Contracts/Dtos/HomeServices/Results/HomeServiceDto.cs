using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Dtos.HomeServices.Results
{
    public class HomeServiceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public List<string> Services { get; set; }
        public List<string> Notes { get; set; }
        public string Image { get; set; }
    }
}
