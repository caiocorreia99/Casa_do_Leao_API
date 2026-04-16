namespace CDL.Models.DataBase
{
    public class Post
    {
        public int IdPost { get; set; }
        public int IdCategory { get; set; }
        public string Title { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
        public string HtmlBody { get; set; } = "";
        public bool Published { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int? AuthorUserId { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<PostCarouselImage> CarouselImages { get; set; } = new List<PostCarouselImage>();
    }
}
