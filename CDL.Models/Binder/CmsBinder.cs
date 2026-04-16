namespace CDL.Models.Binder
{
    public class AgendaItemPublicResponse
    {
        public int IdAgendaItem { get; set; }
        public byte DayOfWeek { get; set; }
        public string Title { get; set; } = null!;
        public string? TimeDisplay { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; } = true;
    }

    public class AgendaItemAdminRequest
    {
        public int? IdAgendaItem { get; set; }
        public byte DayOfWeek { get; set; }
        public string Title { get; set; } = null!;
        public string? TimeDisplay { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; } = true;
    }

    public class CategoryResponse
    {
        public int IdCategory { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    public class CategoryAdminRequest
    {
        public int? IdCategory { get; set; }
        public string Name { get; set; } = null!;
        public string? Slug { get; set; }
        public int SortOrder { get; set; }
    }

    public class PostCarouselImageDto
    {
        public int? IdPostCarouselImage { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int SortOrder { get; set; }
        public string? AltText { get; set; }
    }

    public class PostListItemResponse
    {
        public int IdPost { get; set; }
        public string Title { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
        public int IdCategory { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime? PublishedAt { get; set; }
    }

    public class PostDetailPublicResponse
    {
        public int IdPost { get; set; }
        public int IdCategory { get; set; }
        public string Title { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
        public string HtmlBody { get; set; } = "";
        public bool Published { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<PostCarouselImageDto> CarouselImages { get; set; } = new();
    }

    public class PostAdminRequest
    {
        public int? IdPost { get; set; }
        public int IdCategory { get; set; }
        public string Title { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
        public string HtmlBody { get; set; } = "";
        public bool Published { get; set; }
        public int? AuthorUserId { get; set; }
        public bool Active { get; set; } = true;
        public List<PostCarouselImageDto> CarouselImages { get; set; } = new();
    }

    public class EventRegistrationRowResponse
    {
        public int IdEventRegistration { get; set; }
        public int IdUser { get; set; }
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public DateTime RegisteredAt { get; set; }
    }

    public class MediaUploadResponse
    {
        public string Url { get; set; } = null!;
    }
}
