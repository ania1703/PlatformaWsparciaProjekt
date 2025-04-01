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

        public string SeniorId { get; set; }  // to be linked with Identity in the future
    }
}

