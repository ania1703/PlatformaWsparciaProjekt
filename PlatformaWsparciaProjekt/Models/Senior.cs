namespace PlatformaWsparciaProjekt.Models
{
    public class Senior : User
    {
        public int Age { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }

        public string Password { get; set; }

        public List<Request> Requests { get; set; } = new List<Request>();
    }
}
