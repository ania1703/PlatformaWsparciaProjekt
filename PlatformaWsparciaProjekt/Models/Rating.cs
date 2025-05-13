using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PlatformaWsparciaProjekt.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int? VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }

        [BindNever]
        public int RatedById { get; set; }
        [BindNever] // Dodaj też to!
        public User RatedBy { get; set; } = null!;

        public double Score { get; set; } // Ocena w skali np. 1-5
        public string Comment { get; set; }
        public DateTime RatedAt { get; set; } = DateTime.Now;
        }
}
