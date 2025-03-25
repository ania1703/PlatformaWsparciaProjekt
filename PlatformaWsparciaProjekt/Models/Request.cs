namespace PlatformaWsparciaProjekt.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }

        // Relacja ManyToOne - jedno zgłoszenie przypisane do jednego seniora
        public int SeniorId { get; set; }
        public Senior Senior { get; set; }

        // Relacja ManyToOne - jedno zgłoszenie może być przypisane do jednego wolontariusza
        public int? VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }
    }
}
