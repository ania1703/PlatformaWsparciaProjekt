namespace PlatformaWsparciaProjekt.Models
{
    public class Volunteer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Relacja OneToMany - Wolontariusz może realizować wiele zgłoszeń
        public List<Request> AssignedRequests { get; set; } = new List<Request>();
    }
}
