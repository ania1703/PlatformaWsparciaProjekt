using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PlatformaWsparciaProjekt.Controllers
{
    [Authorize(Roles = "Senior")]
    public class RatingController : Controller
    {
        private readonly AppDbContext _context;

        public RatingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Volunteers = _context.Volunteers.ToList();
            return View(new Rating());
        }

        [HttpGet]

        public IActionResult Index()
        {
            var ratings = _context.Ratings
                .Include(r => r.Volunteer)
                .ToList();

            return View(ratings);
        }

        [HttpPost]
        public IActionResult Create(Rating rating)
        {
            // Ignorujemy RatedBy w walidacji, bo ustawiamy go programowo
            ModelState.Remove(nameof(rating.RatedById));
            ModelState.Remove(nameof(rating.RatedBy));

            if (!ModelState.IsValid)
            {
                ViewBag.Volunteers = _context.Volunteers.ToList();
                return View(rating);
            }

            if (rating.VolunteerId == 0)
            {
                rating.VolunteerId = null;
            }

            rating.RatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            rating.RatedAt = DateTime.Now;

            _context.Ratings.Add(rating);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}