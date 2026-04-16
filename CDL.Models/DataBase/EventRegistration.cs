namespace CDL.Models.DataBase
{
    public class EventRegistration
    {
        public int IdEventRegistration { get; set; }
        public int IdEvent { get; set; }
        public int IdUser { get; set; }
        public DateTime RegisteredAt { get; set; }
        public bool Active { get; set; } = true;

        public Event Event { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
