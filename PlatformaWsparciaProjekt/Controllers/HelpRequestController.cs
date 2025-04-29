using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace PlatformaWsparciaProjekt.Controllers
{
    public class HelpRequestController : Controller
    {
        private readonly AppDbContext _context;

        public HelpRequestController(AppDbContext context)
        {
            _context = context;
        }

        // WYŚWIETLANIE LISTY ZGŁOSZEŃ
        public IActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParam = sortOrder == "date_desc" ? "" : "date_desc";
            ViewBag.PrioritySortParam = sortOrder == "priority" ? "priority_desc" : "priority";

            ViewBag.CurrentFilter = searchString;

            var requests = _context.HelpRequests
                .Include(r => r.Senior)
                .Include(r => r.Volunteer)
                .AsQueryable();

            // 🔍 Filtrowanie po tytule
            if (!string.IsNullOrEmpty(searchString))
            {
                requests = requests.Where(r => r.Title.Contains(searchString));
            }

            // 🔃 Sortowanie
            switch (sortOrder)
            {
                case "date_desc":
                    requests = requests.OrderByDescending(r => r.CreatedAt);
                    break;
                case "priority":
                    requests = requests.OrderBy(r =>
    r.Priority == "High" ? 1 :
    r.Priority == "Medium" ? 2 :
    r.Priority == "Low" ? 3 : 4);

                    break;
                case "priority_desc":
                    requests = requests.OrderByDescending(r => r.Priority);
                    break;
                default:
                    requests = requests.OrderBy(r => r.CreatedAt);
                    break;
            }

            return View(requests.ToList());
        }


        [Authorize(Roles = "Volunteer")]
        public IActionResult MyAssignments()
        {
            // Pobieranie ID zalogowanego wolontariusza
            var volunteerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pobieranie zgłoszeń przypisanych do wolontariusza
            var assignedRequests = _context.HelpRequests
                .Where(hr => hr.VolunteerId == volunteerId)
                .ToList();

            return View(assignedRequests);
        }
       




        // DODAWANIE NOWEGO ZGŁOSZENIA
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HelpRequest helpRequest)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                helpRequest.CreatedByUserId = userId;

                if (userRole == "Senior")
                    helpRequest.SeniorId = userId;
                else if (userRole == "Volunteer")
                    helpRequest.VolunteerId = userId;

                _context.Add(helpRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(helpRequest);
        }


        // EDYCJA ZGŁOSZENIA
        public IActionResult Edit(int id)
        {
            var request = _context.HelpRequests.Include(r => r.Senior).FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if ((userRole == "Senior" && request.SeniorId != userId) ||
                (userRole == "Volunteer" && request.VolunteerId != userId))
            {
                return Forbid();
            }

            return View(request);
        }

        [HttpPost]
        public IActionResult Edit(HelpRequest request)
        {
            var original = _context.HelpRequests.Include(r => r.Senior).FirstOrDefault(r => r.Id == request.Id);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if ((userRole == "Senior" && original.SeniorId != userId) ||
                (userRole == "Volunteer" && original.VolunteerId != userId))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                original.Title = request.Title;
                original.Description = request.Description;
                original.Category = request.Category;
                original.Priority = request.Priority;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(request);
        }

        // USUWANIE ZGŁOSZENIA
        public IActionResult Delete(int id)
        {
            var request = _context.HelpRequests.Include(r => r.Senior).FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if ((userRole == "Senior" && request.SeniorId != userId) ||
                (userRole == "Volunteer" && request.VolunteerId != userId))
            {
                return Forbid();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var request = _context.HelpRequests.Include(r => r.Senior).FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if ((userRole == "Senior" && request.SeniorId != userId) ||
                (userRole == "Volunteer" && request.VolunteerId != userId))
            {
                return Forbid();
            }

            _context.HelpRequests.Remove(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //przypisanie do zgłoszenia
        public async Task<IActionResult> AddVolunteer(int id)
        {
            var request = _context.HelpRequests.Find(id);
            if (request == null) return NotFound();

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Volunteer")
            {
                request.Volunteer = _context.Volunteers.First(x => x.Id == userId);
                request.VolunteerId = userId;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //Funkcja usuwająca przypisanie do zgłoszenia.
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> RemoveVolunteer(int id)
        {
            var request = await _context.HelpRequests
                .Include(r => r.Volunteer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (request.Volunteer == null || request.Volunteer.Id != userId)
            {
                return Forbid(); // tylko przypisany wolontariusz może się wypisać
            }

            request.Volunteer = null;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
