using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlatformaWsparciaProjekt.Data;
using PlatformaWsparciaProjekt.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        private string GetUserRole() => User.FindFirstValue(ClaimTypes.Role);

        public IActionResult Index()
        {
            var allRequests = _context.HelpRequests
                .Include(r => r.Senior)
                .Include(r => r.Volunteer)
                .ToList();

            return View(allRequests);
        }

        // DODAWANIE NOWEGO ZGŁOSZENIA – tylko senior
        [Authorize(Roles = "Senior")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Senior")]
        public async Task<IActionResult> Create(HelpRequest helpRequest)
        {
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                helpRequest.CreatedByUserId = userId;
                helpRequest.SeniorId = userId;

                _context.Add(helpRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(helpRequest);
        }

        // EDYCJA ZGŁOSZENIA – tylko dla właściciela (seniora)
        [Authorize(Roles = "Senior")]
        public IActionResult Edit(int id)
        {
            var request = _context.HelpRequests.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            if (request.SeniorId != GetUserId())
                return Forbid();

            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "Senior")]
        public IActionResult Edit(HelpRequest request)
        {
            var original = _context.HelpRequests.FirstOrDefault(r => r.Id == request.Id);
            if (original == null) return NotFound();

            if (original.SeniorId != GetUserId())
                return Forbid();

            if (ModelState.IsValid)
            {
                original.Title = request.Title;
                original.Description = request.Description;
                original.Category = request.Category;
                original.Priority = request.Priority;

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // USUWANIE ZGŁOSZENIA – tylko dla właściciela (seniora)
        [Authorize(Roles = "Senior")]
        public IActionResult Delete(int id)
        {
            var request = _context.HelpRequests.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            if (request.SeniorId != GetUserId())
                return Forbid();

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Senior")]
        public IActionResult DeleteConfirmed(int id)
        {
            var request = _context.HelpRequests.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            if (request.SeniorId != GetUserId())
                return Forbid();

            _context.HelpRequests.Remove(request);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // PRZYPISANIE WOLONTARIUSZA DO ZGŁOSZENIA
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> AddVolunteer(int id)
        {
            var request = _context.HelpRequests.Find(id);
            if (request == null) return NotFound();

            var userId = GetUserId();
            request.VolunteerId = userId;
            request.Volunteer = _context.Volunteers.FirstOrDefault(v => v.Id == userId);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
