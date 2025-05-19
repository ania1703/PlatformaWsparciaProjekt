using Microsoft.AspNetCore.Mvc;
using PlatformaWsparciaProjekt.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using PlatformaWsparciaProjekt.Models;
using System.ComponentModel.DataAnnotations;

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
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Sprawdzenie czy email lub telefon już istnieje
            bool emailExists = _context.Seniors.Any(s => s.Email == model.Email) ||
                               _context.Volunteers.Any(v => v.Email == model.Email);
            bool phoneExists = _context.Seniors.Any(s => s.Phone == model.Phone) ||
                               _context.Volunteers.Any(v => v.Phone == model.Phone);

            if (emailExists || phoneExists)
            {
                if (emailExists)
                    ModelState.AddModelError("Email", "Użytkownik z tym adresem e-mail już istnieje.");
                if (phoneExists)
                    ModelState.AddModelError("Phone", "Użytkownik z tym numerem telefonu już istnieje.");
                return View(model);
            }

            if (model.Role == "Senior")
            {
                var newSenior = new Senior
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password,
                    Age = 70,
                    Address = "brak",
                    Bio = ""
                };

                _context.Seniors.Add(newSenior);
            }
            else if (model.Role == "Volunteer")
            {
                var newVolunteer = new Volunteer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password
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
            int idUser = senior != null ? senior.Id : volunteer.Id;
            string userName = senior != null ? senior.FirstName : volunteer.FirstName;
            string userRole = senior != null ? "Senior" : "Volunteer";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, idUser.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            TempData["SuccessMessage"] = "Zalogowano pomyślnie.";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
