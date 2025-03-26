namespace PlatformaWsparciaProjekt.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }

        // Lista uczestników (relacja ManyToMany)
        public List<UserEvent> Participants { get; set; } = new List<UserEvent>();
    }

    public class UserEvent
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
