namespace CDL.Models.DataBase
{
    public class Category
    {
        public int IdCategory { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public int SortOrder { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
