namespace CDL.Models.DataBase
{
    public class AgendaItem
    {
        public int IdAgendaItem { get; set; }
        /// <summary>0 = Sunday … 6 = Saturday (System.DayOfWeek)</summary>
        public byte DayOfWeek { get; set; }
        public string Title { get; set; } = null!;
        public string? TimeDisplay { get; set; }
        public int SortOrder { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
