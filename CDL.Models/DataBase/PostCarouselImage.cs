namespace CDL.Models.DataBase
{
    public class PostCarouselImage
    {
        public int IdPostCarouselImage { get; set; }
        public int IdPost { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int SortOrder { get; set; }
        public string? AltText { get; set; }

        public Post Post { get; set; } = null!;
    }
}
