using System.ComponentModel.DataAnnotations;

namespace PlatformaWsparciaProjekt.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Rola")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [Display(Name = "Imie")]

        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres e-mail (np. jan@example.com).")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon jest wymagany")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Numer telefonu musi zawierać dokładnie 9 cyfr")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
    }
}
