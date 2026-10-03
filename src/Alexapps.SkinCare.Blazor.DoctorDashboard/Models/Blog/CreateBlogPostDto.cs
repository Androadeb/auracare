namespace ynex.Models.Blog
{
    public class CreateBlogPostDto
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }

       
        public string Status { get; set; } = "0";

        public string CoverImage { get; set; }
    }
}
