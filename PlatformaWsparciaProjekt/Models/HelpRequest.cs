using System;
using System.ComponentModel.DataAnnotations;

namespace PlatformaWsparciaProjekt.Models
{
    public class HelpRequest
    {
        public int Id { get; set; }
        public int? SeniorId { get; set; }
        public int? VolunteerId { get; set; }
        public int CreatedByUserId { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Category { get; set; }
        public string Priority { get; set; }

        public Senior? Senior { get; set; }
        public Volunteer? Volunteer { get; set; }
    }
}
