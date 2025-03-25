namespace PlatformaWsparciaProjekt.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RatedById { get; set; }
        public User RatedBy { get; set; }

        public double Score { get; set; } // Ocena w skali np. 1-5
        public string Comment { get; set; }
        public DateTime RatedAt { get; set; } = DateTime.Now;
    }
}
