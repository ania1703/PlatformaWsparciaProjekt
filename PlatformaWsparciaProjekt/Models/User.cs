﻿using System.ComponentModel.DataAnnotations;

namespace PlatformaWsparciaProjekt.Models
{
    public abstract class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Pole wspólne do oceny użytkowników
        public double? Rating { get; set; }
    }
}
