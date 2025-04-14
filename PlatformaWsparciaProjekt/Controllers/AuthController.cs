using Microsoft.AspNetCore.Mvc;
using PlatformaWsparciaProjekt.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using PlatformaWsparciaProjekt.Models;

namespace PlatformaWsparciaProjekt.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string role, string firstName, string lastName, string email, string phone, string password)
        {
            if (role == "Senior")
            {
                var newSenior = new Senior
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    Password = password,
                    Age = 70, // możesz też dodać pole formularza dla wieku
                    Address = "brak", // lub dodać jako input
                    Bio = ""
                };

                _context.Seniors.Add(newSenior);
            }
            else if (role == "Volunteer")
            {
                var newVolunteer = new Volunteer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    Password = password
                };

                _context.Volunteers.Add(newVolunteer);
            }

            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Szukamy użytkownika (seniora lub wolontariusza) po emailu i haśle
            var senior = _context.Seniors.FirstOrDefault(u => u.Email == email && u.Password == password);
            var volunteer = _context.Volunteers.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (senior == null && volunteer == null)
            {
                ViewBag.Error = "Nieprawidłowy email lub hasło.";
                return View();
            }

            // Ustalamy dane do roli i imienia użytkownika
            string userName = senior != null ? senior.FirstName : volunteer.FirstName;
            string userRole = senior != null ? "Senior" : "Volunteer";
            string userId = senior != null ? senior.Id.ToString() : volunteer.Id.ToString();


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
