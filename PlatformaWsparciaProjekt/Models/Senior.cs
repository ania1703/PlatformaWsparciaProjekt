namespace PlatformaWsparciaProjekt.Models
{
    public class Senior
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }

        // Relacja OneToMany - Senior może mieć wiele zgłoszeń
        public List<Request> Requests { get; set; } = new List<Request>();
    }
}
