using System.Security.Claims;
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
    [Authorize]
    public class HelpRequestController : Controller
    {
        private readonly AppDbContext _context;


        public HelpRequestController(AppDbContext context)
        {
            _context = context;
        }

        // WYŚWIETLANIE LISTY ZGŁOSZEŃ
        public IActionResult Index()
        {
            var requests = _context.HelpRequests.Include(x=>x.Senior).Include(x=>x.Volunteer).ToList();
            return View(requests);
        }

        // DODAWANIE NOWEGO ZGŁOSZENIA    var helpRequests = _context.HelpRequests

            // Pobieramy wszystkie zgłoszenia
            var allRequests = _context.HelpRequests
                .Include(r => r.Senior)
                .Include(r => r.Volunteer)
                .ToList();

            return View(allRequests);
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

                var userName = HttpContext.User.Identity.Name;
                var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                // <--- przypisz zalogowanego usera

                if (userRole == "Senior")
                {
                    helpRequest.Senior = _context.Seniors.First(x=>x.Id == userId);

                }
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

        public async Task<IActionResult> AddVolunteer(int id)
        {
            var request = _context.HelpRequests.Find(id);

            if (request == null)
            {
                return NotFound();
            }

            var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // PRZYPISANIE WOLONTARIUSZA DO ZGŁOSZENIA
        public async Task<IActionResult> AddVolunteer(int id)
        {
            var request = _context.HelpRequests.Find(id);
            if (request == null) return NotFound();

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Volunteer")
            {
                request.Volunteer = _context.Volunteers.First(x => x.Id == userId);

            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

                request.VolunteerId = userId;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

