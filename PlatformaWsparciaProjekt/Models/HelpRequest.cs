using System;
using System.ComponentModel.DataAnnotations;

namespace PlatformaWsparciaProjekt.Models
{
    public class HelpRequest
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Senior? Senior { get; set; }  // pole opcjonalne
                                               // to be linked with Identity in the future
        public string Category { get; set; } // Kategoria pomocy (np. zakupy, towarzystwo)
        public string Priority { get; set; } // Priorytet pomocy (np. wysoki, średni, niski)

        public Volunteer? Volunteer { get; set; }
    }
}

