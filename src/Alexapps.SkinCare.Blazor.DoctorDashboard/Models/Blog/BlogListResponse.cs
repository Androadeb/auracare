namespace ynex.Models.Blog
{
    public class BlogListResponse
    {
        public int TotalCount { get; set; }
        public List<BlogModel> Items { get; set; }
    }
}
