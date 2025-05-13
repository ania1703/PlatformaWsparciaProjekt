using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PlatformaWsparciaProjekt.ViewModels;

namespace PlatformaWsparciaProjekt.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly AppDbContext _context;

        public RatingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Senior")]
        public IActionResult Create()
        {
            ViewBag.Volunteers = _context.Volunteers.ToList();
            return View(new Rating());
        }

        [HttpGet]


        [HttpGet]
        public IActionResult Index()
        {
            var volunteersWithRatings = _context.Volunteers
                .Where(v => _context.Ratings.Any(r => r.VolunteerId == v.Id))
                .Select(v => new VolunteerRatingSummaryViewModel
                {
                    VolunteerId = v.Id,
                    FirstName = v.FirstName,
                    LastName = v.LastName,
                    AverageScore = Math.Round(
                        _context.Ratings
                            .Where(r => r.VolunteerId == v.Id)
                            .Average(r => r.Score), 2)
                })
                .ToList();

            return View(volunteersWithRatings);
        }


        public IActionResult Details(int volunteerId)
        {
            var ratings = _context.Ratings
                .Where(r => r.VolunteerId == volunteerId)
                .Include(r => r.RatedBy)
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

        [HttpGet]
        public IActionResult VolunteerRatingsSummary()
        {
            var ratings = _context.Ratings
                .Include(r => r.Volunteer)
                .Where(r => r.VolunteerId != null)
                .ToList();

            var summary = ratings
                .GroupBy(r => r.VolunteerId)
                .Select(g => new
                {
                    Volunteer = g.First().Volunteer!,
                    AverageScore = Math.Round(g.Average(r => r.Score), 2),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.AverageScore)
                .ToList();

            return View(summary);
        }

        [HttpGet]
        public IActionResult VolunteerDetails(int id)
        {
            var volunteer = _context.Volunteers.FirstOrDefault(v => v.Id == id);
            if (volunteer == null) return NotFound();

            var ratings = _context.Ratings
                .Where(r => r.VolunteerId == id)
                .OrderByDescending(r => r.RatedAt)
                .ToList();

            ViewBag.Volunteer = volunteer;
            return View(ratings);
        }

        [HttpGet]
        [Authorize(Roles = "Senior")]
        public IActionResult Edit(int id)
        {
            var rating = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null || rating.RatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            return View(rating);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Senior")]
        public IActionResult Edit(int id, Rating updatedRating)
        {
            var rating = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null || rating.RatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            rating.Score = updatedRating.Score;
            rating.Comment = updatedRating.Comment;

            _context.SaveChanges();
            return RedirectToAction("VolunteerDetails", new { id = rating.VolunteerId });
        }

        [HttpGet]
        [Authorize(Roles = "Senior")]
        public IActionResult Delete(int id)
        {
            var rating = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null || rating.RatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            return View(rating);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Senior")]
        public IActionResult DeleteConfirmed(int id)
        {
            var rating = _context.Ratings.FirstOrDefault(r => r.Id == id);
            if (rating == null || rating.RatedById != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))
                return Forbid();

            _context.Ratings.Remove(rating);
            _context.SaveChanges();
            return RedirectToAction("VolunteerDetails", new { id = rating.VolunteerId });
        }

    }
}