namespace PlatformaWsparciaProjekt.Models
{
    public class Volunteer : User
    {
        public string Password { get; set; }

        public List<Request> AssignedRequests { get; set; } = new List<Request>();
    }
}
