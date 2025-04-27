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

            var rating = new Rating();

            // 🟢 możesz ustawić wartości domyślne, np. rating.UserId = null; ale to i tak domyślne dla nullable

            return View(rating);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var ratings = _context.Ratings // ✔️ nazwa DbSetu
     .Include(r => r.User)
     .Include(r => r.RatedBy)
     .ToList();


            return View(ratings);
        }


        [HttpPost]
        public IActionResult Create(Rating rating)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Volunteers = _context.Volunteers.ToList();
                return View(rating);
            }

            if (rating.UserId == 0)
            {
                rating.UserId = null;
            }

            rating.RatedById = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            rating.RatedAt = DateTime.Now;

            _context.Ratings.Add(rating);
            _context.SaveChanges();

            // 🔁 Po zapisaniu wracamy do listy ocen
            return RedirectToAction("Index", "Rating");
        }

    }
}
